using System.Linq;
using System.Text;
using VT100.Attributes;
using VT100.Utilities.ReadConsole;

namespace VT100.Tests.Fakes
{
    public static class RawTextExtractor
    {
        public static string Extract(string input)
        {
            var elements = ControlElementExtractor.ListFromString(input);
            var sequences = AnsiRecognition.Split(elements);
            var sb = new StringBuilder();

            foreach (var sequence in sequences)
            {
                if (sequence.CodeType == AnsiCodeType.None)
                    sb.Append(string.Concat(sequence.Items.Select(i => i.KeyChar)));
            }

            return sb.ToString();
        }
    }
}