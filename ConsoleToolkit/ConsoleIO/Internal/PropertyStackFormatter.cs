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
        /// <param name="value">The value that will be combined with the heading specified in the <see cref="format"/>.</param>
        /// <param name="columnWidth">The width to which the column should be formatted.</param>
        /// <returns>An array of lines containing the formatted output.</returns>
        public static string[] Format(ColumnFormat format, string value, int columnWidth, int tabLength = 4)
        {
            var words = WordSplitter.Split(value, tabLength);
            var firstLine = string.Format("{0}: ", format.Heading);

            var headingText = ColumnWrapper.WrapValue(firstLine, new ColumnFormat(null, typeof (string)), columnWidth);
            var lastHeadingLineLength = headingText.Last().Length;

            var numWordsForFirstLine = CountWordsThatFitFirstLine(lastHeadingLineLength, columnWidth, words);

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
            var rest = ColumnWrapper.WrapAndMeasureWords(words.Skip(numWordsForFirstLine), format, columnWidth, out wrappedLines);
            if (format.Alignment == ColumnAlign.Right)
                rest = RightAlign(columnWidth, rest);

            return headingText.Concat(rest).ToArray();
        }

        private static string[] RightAlign(int columnWidth, IEnumerable<string> lines)
        {
            return lines.Select(l => string.Format("{0," + columnWidth + "}", l)).ToArray();
        }

        private static string[] WrapFullValue(string firstLine, SplitWord[] words, ColumnFormat format, int columnWidth, int tabLength)
        {
            var firstLineWords = WordSplitter.Split(firstLine, tabLength);
            int wrappedLines;
            var lines = ColumnWrapper.WrapAndMeasureWords(firstLineWords.Concat(words), format, columnWidth, out wrappedLines);

            if (format.Alignment == ColumnAlign.Right)
                return RightAlign(columnWidth, lines);

            return lines;
        }

        private static string ConcatenateWords(IEnumerable<SplitWord> words)
        {
            var output = string.Empty;
            var wordEnumerator = words.GetEnumerator();
            var valid = wordEnumerator.MoveNext();
            while (valid)
            {
                var current = wordEnumerator.Current;
                output += current.WordValue;
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