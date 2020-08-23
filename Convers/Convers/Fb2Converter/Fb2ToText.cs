using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace Convers
{
    public class Fb2ToText : IToTextConverter
    {
        private List<string> LoadFile(string path)
        {
            var e = XElement.Load(path);
            var name = e.Name;

            var pname = XName.Get("p", name.NamespaceName);

            var lines = e.Descendants(pname);
            var result = new List<string>();

            foreach (var line in lines)
            {
                result.Add(line.Value);
            }

            return result;
        }

        public StringBuilder ToText(string path)
        {
            var lines = LoadFile(path);
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                result.Append(line);
                result.Append("\n");
            }

            return result;
        }
        public IEnumerable<string> ToStrings(string path)
        {
            return LoadFile(path);
        }
        public IEnumerable<string[]> ToStringsParts(string path)
        {
            var lines = LoadFile(path);
            var result = new List<string[]>();

            foreach (var line in lines)
            {
                result.Add(line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }

            return result;
        }
    }
}
