using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convers
{
    public interface IToTextConverter
    {
        StringBuilder ToText(string path);
        IEnumerable<string> ToStrings(string path);
        IEnumerable<string[]> ToStringsParts(string path);
    }
    public static class Converter
    {
        public static PdfConverter PdfConverter
        {
            get
            {
                return new PdfConverter();
            }
        }
        public static Fb2Converter Fb2Converter
        {
            get
            {
                return new Fb2Converter();
            }
        }
    }
}
