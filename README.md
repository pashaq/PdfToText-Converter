# PdfToText-Converter
Converting the Pdf and Fb2 documents to text or to the list of articles.

Using open source itext7.

Dependence as dependence itext7.

Correct articles definition.

One line to convert:
```
using System.IO;
using Convers;

namespace Convers.test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var lines = Converter.PdfConverter.ToStrings("d:/pdf.pdf");
            var text = Converter.PdfConverter.ToText("d:/pdf.pdf");

            using (var writer = File.CreateText("d:/pdf.txt"))
            {
                writer.Write(text);
            }
            
            var text1 = Converter.Fb2Converter.ToText("d:/fb2.fb2");

            using (var writer = File.CreateText("d:/fb2.txt"))
            {
                writer.Write(text);
            }
        }
    }
}
```
