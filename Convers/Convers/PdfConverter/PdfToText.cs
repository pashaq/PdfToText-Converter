using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Convers
{
    public struct PdfPageText
    {
        public readonly SortedSet<int> Verticals;
        public readonly List<PdfTextLine> Lines;
        public PdfPageText(SortedSet<int> verticals, List<PdfTextLine> lines)
        {
            Verticals = verticals;
            Lines = lines;
        }
    }
    public class PdfToText : IToTextConverter
    {
        public SortedSet<int> Verticals = new SortedSet<int>();
        public List<PdfPageText> Pages = new List<PdfPageText>();

        public StringBuilder ToText(string path)
        {
            Convert(path);
            return CreateText();
        }
        public IEnumerable<string> ToStrings(string path)
        {
            Convert(path);
            return CreateLines();
        }
        public IEnumerable<string[]> ToStringsParts(string path)
        {
            Convert(path);
            return CreateLinesToParts();
        }

        private void Convert(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                var pdfDocument = new PdfDocument(reader);
                int n = pdfDocument.GetNumberOfPages();

                for (int i = 1; i <= n; i++)
                {
                    var page = pdfDocument.GetPage(i);

                    if (page == null)
                    {
                        break;
                    }

                    var size = page.GetPageSize();
                    var strategy = new TextExtractStrategy(size);

                    PdfTextExtractor.GetTextFromPage(page, strategy);

                    Verticals.AddAll(strategy.Verticals);
                    Pages.Add(new PdfPageText(strategy.Verticals, strategy.Lines));
                }
            }
        }
        private List<string[]> CreateLinesToParts()
        {
            var result = new List<string[]>();
            var article = new StringBuilder();

            foreach (var page in Pages)
            {
                var verticals = page.Verticals;
                var lines = page.Lines;

                if (lines.Count == 0)
                {
                    continue;
                }

                foreach (var line in lines)
                {
                    int space = line.Space;
                    string text = line.String;

                    if (text.Trim().Length == 0)
                    {
                        NewLine();
                        continue;
                    }

                    if (space > Verticals.First())
                    {
                        NewLine();
                    }
                    article.Append(text);
                }
            }
            void NewLine()
            {
                if (article.Length > 0)
                {
                    result.Add(article.ToString().Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    article.Clear();
                }
            }
            return result;
        }
        private List<string> CreateLines()
        {
            var result = new List<string>();
            var article = new StringBuilder();

            foreach (var page in Pages)
            {
                var verticals = page.Verticals;
                var lines = page.Lines;

                if (lines.Count == 0)
                {
                    continue;
                }

                foreach (var line in lines)
                {
                    int space = line.Space;
                    string text = line.String;

                    if (text.Trim().Length == 0)
                    {
                        NewLine();
                        continue;
                    }

                    if (space > Verticals.First())
                    {
                        NewLine();
                    }
                    article.Append(text);
                }
            }
            void NewLine()
            {
                if (article.Length > 0)
                {
                    article.Replace('\t', ' ').Replace("  ", " ");
                    result.Add(article.ToString());
                    article.Clear();
                }
            }
            return result;
        }
        private StringBuilder CreateText()
        {
            var result = new StringBuilder();
            var article = new StringBuilder();

            foreach (var page in Pages)
            {
                var verticals = page.Verticals;
                var lines = page.Lines;

                if (lines.Count == 0)
                {
                    continue;
                }

                foreach (var line in lines)
                {
                    int space = line.Space;
                    string text = line.String;

                    if (text.Trim().Length == 0)
                    {
                        NewLine();
                        continue;
                    }

                    if (space > Verticals.First())
                    {
                        NewLine();
                    }
                    article.Append(text);
                }
            }
            void NewLine()
            {
                if (article.Length > 0)
                {
                    article.Replace('\t', ' ').Replace("  ", " ").Append('\n');
                    result.Append(article.ToString());
                    article.Clear();
                }
            }
            return result;
        }
    }
}
