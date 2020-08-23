using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Convers;

namespace Convers.test
{
    class Program
    {

        static void Main(string[] args)
        {

            var conversTest = new ConversTest();

            var text = conversTest.Fb2ToText(d:/fb2.fb2");

            using (var writer = File.CreateText("d:/fb2.txt"))
            {
                writer.Write(text);
            }            

            var text1 = conversTest.PdfToText(d:/pdf.pdf");

            using (var writer = File.CreateText("d:/pdf.txt"))
            {
                writer.Write(text1);
            }
        }
    }
}
