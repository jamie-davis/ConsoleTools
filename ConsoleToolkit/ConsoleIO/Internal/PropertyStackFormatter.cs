using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// This class is used to format values for display in a stack column. These are columns used to 
    /// replace normal columns when there is insufficient horizontal space to show all of the data 
    /// in a normal table.
    /// 
    /// For example: 
    /// <code>
    /// A     B       C
    /// ----- ------- -------
    /// aaaaa bbbbbbb ccccccc DDDDD: dddddd
    /// aa a  bbbbb           EEEE: eeee ee
    /// a                     eee eee eee
    ///                       FFFFFFF: ffff
    /// </code>
    /// The values for A, B, and C are displayed normally, but D, E and F have been "stacked".
    /// </summary>
    internal static class PropertyStackFormatter
    {
        /// <summary>
        /// Use the format information to return a wrapped and formatted value for a property stack.
        /// </summary>
        /// <param name="format">The format information that will be used to perform the formatting.</param>
        /// <param name="valueObject">The value that will be combined with the heading specified in the <see cref="format"/>.</param>
        /// <param name="columnWidth">The width to which the column should be formatted.</param>
        /// <param name="tabLength">The number of spaces represented by tab characters.</param>
        /// <param name="firstLineHangingIndent">Number of characters at the start of the first line that cannot be used for 
        /// wrapping. This is required when the wrapped text begins part way along an existing line.</param>
        /// <returns>An array of lines containing the formatted output.</returns>
        public static string[] Format(ColumnFormat format, object valueObject, int columnWidth, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            if (valueObject is IConsoleRenderer)
                return FormatRenderer(format, valueObject as IConsoleRenderer, columnWidth, tabLength, firstLineHangingIndent);

            return FormatStringValue(format, valueObject as string, columnWidth, tabLength, firstLineHangingIndent);
        }

        private static string[] FormatRenderer(ColumnFormat format, IConsoleRenderer value, int columnWidth, int tabLength, int firstLineHangingIndent)
        {
            var headingText = MakeHeading(format, columnWidth, tabLength, firstLineHangingIndent);
            int wrappedLines;
            var renderedData = value.Render(columnWidth, out wrappedLines);
            return headingText.Concat(renderedData).ToArray();
        }

        private static string[] MakeHeading(ColumnFormat format, int columnWidth, int tabLength, int firstLineHangingIndent)
        {
            var firstLine = string.Format("{0}: ", format.Heading);
            return ColumnWrapper.WrapValue(firstLine, new ColumnFormat(null, typeof(string)), columnWidth, tabLength,
                firstLineHangingIndent);
        }

        private static string[] FormatStringValue(ColumnFormat format, string value, int columnWidth, int tabLength,
            int firstLineHangingIndent)
        {
            var words = WordSplitter.Split(value, tabLength);

            var headingText = MakeHeading(format, columnWidth, tabLength, firstLineHangingIndent);
            var lastHeadingLineLength = headingText.Last().Length;

            var firstLineEffectiveWidth = columnWidth - (headingText.Count() > 1 ? 0 : firstLineHangingIndent);
            var numWordsForFirstLine = CountWordsThatFitFirstLine(lastHeadingLineLength, firstLineEffectiveWidth, words);

            if (numWordsForFirstLine > 0)
            {
                var firstLineWords = ConcatenateWords(words.Take(numWordsForFirstLine));
                var spaceAvailable = columnWidth - lastHeadingLineLength;
                if (firstLineWords.Length < spaceAvailable)
                    firstLineWords = new string(' ', spaceAvailable - firstLineWords.Length) + firstLineWords;
                headingText[headingText.Length - 1] = headingText[headingText.Length - 1] + firstLineWords;
            }

            if (words.Length == numWordsForFirstLine)
                return headingText;

            int wrappedLines;
            var rest = ColumnWrapper.WrapAndMeasureWords(words.Skip(numWordsForFirstLine), format, columnWidth, 0,
                out wrappedLines);
            if (format.Alignment == ColumnAlign.Right)
                rest = RightAlign(columnWidth, rest);

            return headingText.Concat(rest).ToArray();
        }

        private static string[] RightAlign(int columnWidth, IEnumerable<string> lines)
        {
            return lines.Select(l =>
            {
                var format = "{0," + columnWidth + "}";
                return string.Format(format, l);
            }).ToArray();
        }

        private static string ConcatenateWords(IEnumerable<SplitWord> words)
        {
            var output = string.Empty;
            var wordEnumerator = words.GetEnumerator();
            var valid = wordEnumerator.MoveNext();
            while (valid)
            {
                var current = wordEnumerator.Current;
                output += current.GetWordValue();
                valid = wordEnumerator.MoveNext();
                if (valid)
                    output += new string(' ', current.TrailingSpaces);
            }

            return output;
        }

        private static int CountWordsThatFitFirstLine(int headingLength, int columnWidth, IEnumerable<SplitWord> words)
        {
            var remaining = columnWidth - headingLength;
            var wordsSoFar = 0;
            var wordEnumerator = words.GetEnumerator();

            while (wordEnumerator.MoveNext() && remaining >= wordEnumerator.Current.Length)
            {
                ++wordsSoFar;
                remaining -= wordEnumerator.Current.Length + wordEnumerator.Current.TrailingSpaces;
            }

            return wordsSoFar;
        }
    }
}