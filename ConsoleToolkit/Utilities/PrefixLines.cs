using System;
using System.Text;

namespace ConsoleToolkit.Utilities
{
    /// <summary>
    /// Attach a prefix to each line in a set of lines contained in a string.
    /// </summary>
    internal static class PrefixLines
    {
        internal static string Do(string lines, string prefix)
        {
            if (string.IsNullOrEmpty(lines)) return lines;

            var sb = new StringBuilder();

            var pos = 0;
            var eol = 0;
            var eolLen = Environment.NewLine.Length;
            while ((eol = lines.IndexOf(Environment.NewLine, pos, StringComparison.Ordinal)) >= 0)
            {
                sb.Append(prefix);
                sb.Append(lines.Substring(pos, eol - pos + eolLen));
                pos = eol + eolLen;
            }

            if (pos < lines.Length)
            {
                sb.Append(prefix);
                sb.Append(lines.Substring(pos));
            }

            return sb.ToString();
        }
    }
}
