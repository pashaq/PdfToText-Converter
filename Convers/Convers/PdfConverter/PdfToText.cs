using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class PdfToText
    {
        public static SortedSet<int> Verticals = new SortedSet<int>();
        public static List<PdfPageText> Pages = new List<PdfPageText>();

        public static List<string> Convert(string path)
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

                return CreateLines();
            }
        }
        private static List<string> CreateLines()
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
                    result.Add(article.ToString());
                    article.Clear();
                }
            }
            return result;
        }
    }
}
