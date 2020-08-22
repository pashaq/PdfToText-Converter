using System.IO;
using Convers;

namespace Convers.test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var lines = Converter.PdfConverter.ToText("d:/pdf.pdf");
            var text = Converter.PdfConverter.ToText("d:/pdf.pdf");

            text = text.Replace('\t', ' ');

            using (var writer = File.CreateText("d:/pdf.txt"))
            {
                writer.Write(text);
            }
        }
    }
}
