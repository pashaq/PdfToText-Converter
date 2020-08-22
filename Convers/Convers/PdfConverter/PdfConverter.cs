using System.Collections.Generic;
using System.Text;

namespace Convers
{
    public class PdfConverter
    {
        public StringBuilder ToText(string path)
        {
            var lines = PdfToText.Convert(path);
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                result.Append(line);
                result.Append("\n");
            }

            return result;
        }
        public List<string> ToStrings(string path)
        {
            return PdfToText.Convert(path);
        }

    }
}
