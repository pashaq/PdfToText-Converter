using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convers
{
    public class Fb2Converter : IToTextConverter
    {
        public StringBuilder ToText(string path)
        {
            var conv = new Fb2ToText();
            return conv.ToText(path);
        }
        public IEnumerable<string> ToStrings(string path)
        {
            var conv = new Fb2ToText();
            return conv.ToStrings(path);
        }
        public IEnumerable<string[]> ToStringsParts(string path)
        {
            var conv = new Fb2ToText();
            return conv.ToStringsParts(path);
        }
    }
}
