using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convers
{
    public class PdfConverter : IToTextConverter
    {
        public StringBuilder ToText(string path)
        {
            var conv = new PdfToText();
            return conv.ToText(path);
        }
        public IEnumerable<string> ToStrings(string path)
        {
            var conv = new PdfToText();
            return conv.ToStrings(path);
        }
        public IEnumerable<string[]> ToStringsParts(string path)
        {
            var conv = new PdfToText();
            return conv.ToStringsParts(path);
        }

    }
}
