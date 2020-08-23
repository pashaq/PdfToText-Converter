using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convers;

namespace Convers.test
{
    public class ConversTest
    {
        public StringBuilder PdfToText(string path)
        {
            return Converter.PdfConverter.ToText(path);
        }
        public IEnumerable<string> PdfToStrings(string path)
        {
            return Converter.PdfConverter.ToStrings(path);
        }
        public IEnumerable<string[]> PdfToStringsParts(string path)
        {
            return Converter.PdfConverter.ToStringsParts(path);
        }

        public StringBuilder Fb2ToText(string path)
        {
            return Converter.Fb2Converter.ToText(path);
        }
        public IEnumerable<string> Fb2ToStrings(string path)
        {
            return Converter.Fb2Converter.ToStrings(path);
        }
        public IEnumerable<string[]> Fb2ToStringsParts(string path)
        {
            return Converter.Fb2Converter.ToStringsParts(path);
        }
    }
}
