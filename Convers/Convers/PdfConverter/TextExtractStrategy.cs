/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Convers
{
    public struct PdfTextLine
    {
        public readonly int Space;
        public readonly string String;
        public PdfTextLine(int space, string str)
        {
            Space = space;
            String = str;
        }
    }
    public class TextExtractStrategy : ITextExtractionStrategy
    {
        private readonly IList<TextChunk> locationalResult = new List<TextChunk>();

        private readonly ITextChunkLocationStrategy tclStrat;

        private bool useActualText = false;

        private bool rightToLeftRunDirection = false;

        private TextRenderInfo lastTextRenderInfo;

        private Rectangle pageSize;

        public List<PdfTextLine> Lines { get; private set; } = new List<PdfTextLine>();
        public SortedSet<int> Verticals { get; private set; } = new SortedSet<int>();

        public TextExtractStrategy(Rectangle pageSize)
            : this(new _ITextChunkLocationStrategy_85())
        {
            this.pageSize = pageSize;
        }

        private sealed class _ITextChunkLocationStrategy_85 : ITextChunkLocationStrategy
        {
            public _ITextChunkLocationStrategy_85()
            {
            }

            public ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline)
            {
                return new TextChunkLocationDefaultImp(baseline.GetStartPoint(), baseline.GetEndPoint(), renderInfo.GetSingleSpaceWidth
                    ());
            }
        }

        public TextExtractStrategy(ITextChunkLocationStrategy strat)
        {
            tclStrat = strat;
        }

        public virtual TextExtractStrategy SetUseActualText(bool useActualText)
        {
            this.useActualText = useActualText;
            return this;
        }

        public virtual TextExtractStrategy SetRightToLeftRunDirection(bool rightToLeftRunDirection)
        {
            this.rightToLeftRunDirection = rightToLeftRunDirection;
            return this;
        }

        public virtual bool IsUseActualText()
        {
            return useActualText;
        }

        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type.Equals(EventType.RENDER_TEXT))
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                LineSegment segment = renderInfo.GetBaseline();
                if (renderInfo.GetRise() != 0)
                {
                    // remove the rise from the baseline - we do this because the text from a super/subscript render operations should probably be considered as part of the baseline of the text the super/sub is relative to
                    Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
                    segment = segment.TransformBy(riseOffsetTransform);
                }
                if (useActualText)
                {
                    CanvasTag lastTagWithActualText = lastTextRenderInfo != null ? 
                        FindLastTagWithActualText(lastTextRenderInfo.GetCanvasTagHierarchy()) : null;

                    if (lastTagWithActualText != null && lastTagWithActualText == 
                        FindLastTagWithActualText(renderInfo.GetCanvasTagHierarchy()))
                    {
                        // Merge two text pieces, assume they will be in the same line
                        TextChunk lastTextChunk = locationalResult[locationalResult.Count - 1];

                        Vector mergedStart = new Vector(
                            Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(0), 
                                segment.GetStartPoint().Get(0)), 
                            Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(1), 
                                segment.GetStartPoint().Get(1)), 
                            Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(2), 
                                segment.GetStartPoint().Get(2)));

                        Vector mergedEnd = new Vector(
                            Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(0), 
                                segment.GetEndPoint().Get(0)), 
                            Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(1), 
                                segment.GetEndPoint().Get(1)), 
                            Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(2), 
                                segment.GetEndPoint().Get(2)));

                        TextChunk merged = new TextChunk(
                            lastTextChunk.GetText(), 
                            tclStrat.CreateLocation(renderInfo, 
                                new LineSegment(mergedStart, mergedEnd)));

                        locationalResult[locationalResult.Count - 1] = merged;
                    }
                    else
                    {
                        string actualText = renderInfo.GetActualText();

                        TextChunk tc = new TextChunk(
                            actualText ?? renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));
                        
                        locationalResult.Add(tc);
                    }
                }
                else
                {
                    TextChunk tc = new TextChunk(
                        renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));

                    locationalResult.Add(tc);
                }
                lastTextRenderInfo = renderInfo;
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents()
        {
            return null;
        }

        public virtual string GetResultantText()
        {
            var textChunks = new List<TextChunk>(locationalResult);

            SortWithMarks(textChunks);

            var line = new StringBuilder();
            int space = 0;

            TextChunk lastChunk = null;

            foreach (TextChunk chunk in textChunks)
            {
                if (lastChunk == null)
                {
                    space = (int)Math.Round(chunk.GetLocation().GetStartLocation().Get(0));
                    Verticals.Add(space);

                    line.Append(chunk.text);
                }
                else
                {
                    if (chunk.SameLine(lastChunk))
                    {
                        // we only insert a blank space if the trailing character of the previous string 
                        //wasn't a space, and the leading character of the current string isn't a space
                        if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.text) 
                                && !EndsWithSpace(lastChunk.text))
                        {
                            line.Append(' ');
                        }

                        line.Append(chunk.text);
                    }
                    else
                    {
                        Lines.Add(new PdfTextLine(space, line.ToString()));

                        space = (int)Math.Round(chunk.GetLocation().GetStartLocation().Get(0));
                        Verticals.Add(space);

                        line.Clear();

                        line.Append(chunk.text);
                    }
                }
                lastChunk = chunk;
            }

            if (line.Length > 0)
            {
                Lines.Add(new PdfTextLine(space, line.ToString()));
            }

            return null;
        }

        protected internal virtual bool IsChunkAtWordBoundary(TextChunk chunk, TextChunk previousChunk)
        {
            return chunk.GetLocation().IsAtWordBoundary(previousChunk.GetLocation());
        }

        private bool StartsWithSpace(string str)
        {
            return str.Length != 0 && str[0] == ' ';
        }

        private bool EndsWithSpace(string str)
        {
            return str.Length != 0 && str[str.Length - 1] == ' ';
        }

        private CanvasTag FindLastTagWithActualText(IList<CanvasTag> canvasTagHierarchy)
        {
            CanvasTag lastActualText = null;
            foreach (CanvasTag tag in canvasTagHierarchy)
            {
                if (tag.GetActualText() != null)
                {
                    lastActualText = tag;
                    break;
                }
            }
            return lastActualText;
        }

        private void SortWithMarks(IList<TextChunk> textChunks)
        {
            IDictionary<TextChunk, TextChunkMarks> marks = 
                            new Dictionary<TextChunk, TextChunkMarks>();
            IList<TextChunk> toSort = new List<TextChunk>();

            for (int markInd = 0; markInd < textChunks.Count; markInd++)
            {
                ITextChunkLocation location = textChunks[markInd].GetLocation();

                if (location.GetStartLocation().Equals(location.GetEndLocation()))
                {
                    bool foundBaseToAttachTo = false;

                    for (int baseInd = 0; baseInd < textChunks.Count; baseInd++)
                    {
                        if (markInd != baseInd)
                        {
                            ITextChunkLocation baseLocation = textChunks[baseInd].GetLocation();

                            if (!baseLocation.GetStartLocation().Equals(baseLocation.GetEndLocation()) 
                                && TextChunkLocationDefaultImp.ContainsMark(baseLocation, location))
                            {
                                TextChunkMarks currentMarks = marks.Get(textChunks[baseInd]);

                                if (currentMarks == null)
                                {
                                    currentMarks = new TextChunkMarks();
                                    marks.Put(textChunks[baseInd], currentMarks);
                                }
                                if (markInd < baseInd)
                                {
                                    currentMarks.preceding.Add(textChunks[markInd]);
                                }
                                else
                                {
                                    currentMarks.succeeding.Add(textChunks[markInd]);
                                }

                                foundBaseToAttachTo = true;
                                break;
                            }
                        }
                    }

                    if (!foundBaseToAttachTo)
                    {
                        toSort.Add(textChunks[markInd]);
                    }
                }
                else
                {
                    toSort.Add(textChunks[markInd]);
                }
            }

            JavaCollectionsUtil.Sort(toSort, new TextChunkLocationBasedComparator(
                                new DefaultTextChunkLocationComparator(!rightToLeftRunDirection)));

            textChunks.Clear();

            foreach (TextChunk current in toSort)
            {
                TextChunkMarks currentMarks = marks.Get(current);

                if (currentMarks != null)
                {
                    if (!rightToLeftRunDirection)
                    {
                        for (int j = 0; j < currentMarks.preceding.Count; j++)
                        {
                            textChunks.Add(currentMarks.preceding[j]);
                        }
                    }
                    else
                    {
                        for (int j = currentMarks.succeeding.Count - 1; j >= 0; j--)
                        {
                            textChunks.Add(currentMarks.succeeding[j]);
                        }
                    }
                }
                textChunks.Add(current);

                if (currentMarks != null)
                {
                    if (!rightToLeftRunDirection)
                    {
                        for (int j = 0; j < currentMarks.succeeding.Count; j++)
                        {
                            textChunks.Add(currentMarks.succeeding[j]);
                        }
                    }
                    else
                    {
                        for (int j = currentMarks.preceding.Count - 1; j >= 0; j--)
                        {
                            textChunks.Add(currentMarks.preceding[j]);
                        }
                    }
                }
            }
        }

        public interface ITextChunkLocationStrategy
        {
            ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline);
        }

        private class TextChunkMarks
        {
            internal IList<TextChunk> preceding = new List<TextChunk>();

            internal IList<TextChunk> succeeding = new List<TextChunk>();
        }
    }
}
