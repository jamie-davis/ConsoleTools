using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// A static class responsible for wrapping values into a column.
    /// </summary>
    internal static class ColumnWrapper
    {
        /// <summary>
        /// Given a value for a column, wrap the value into as many lines as required.
        /// </summary>
        /// <param name="value">The value to be displayed in the column.</param>
        /// <param name="format">The column's format specification.</param>
        /// <param name="columnWidth">The width allowed for the column.</param>
        /// <param name="tabLength">The number of spaces tabs should represent. Please note that actual tabstops are not supported.</param>
        /// <returns>An array of one or more lines.</returns>
        public static string[] WrapValue(object value, ColumnFormat format, int columnWidth, int tabLength = 4)
        {
            Debug.Assert(columnWidth > 0);
            int wrappedLines;
            return WrapAndMeasureValue(value, format, columnWidth, tabLength, out wrappedLines);
        }

        /// <summary>
        /// Given a value for a column, calculate the number of line breaks that need to be added to wrap the value.
        /// 
        /// Hard line breaks in the value are not counted because they are part of the data and cannot be avoided. The 
        /// function calculates the effect of setting the column width to a specific value.
        /// 
        /// The function will return zero  if no linebreaks need to be added. This will be the case if the value is small
        /// enough that it does not need to be wrapped.
        /// </summary>
        /// <param name="value">The value to be processed.</param>
        /// <param name="format">The column's format specification.</param>
        /// <param name="columnWidth">The width allowed for the column.</param>
        /// <param name="tabLength">The number of spaces tabs should represent. Please note that actual tabstops are not supported.</param>
        /// <returns>The number of added line breaks.</returns>
        public static int CountWordwrapLineBreaks(object value, ColumnFormat format, int columnWidth, int tabLength = 4)
        {
            int wrappedLines;
            WrapAndMeasureValue(value, format, columnWidth, tabLength, out wrappedLines);
            return wrappedLines;
        }

        /// <summary>
        /// Given a value for a column, wrap the value into as many lines as required and calculate the number of line breaks
        /// that need to be added.
        /// </summary>
        /// <param name="value">The value to be displayed in the column.</param>
        /// <param name="format">The column's format specification.</param>
        /// <param name="columnWidth">The width allowed for the column.</param>
        /// <param name="tabLength">The number of spaces tabs should represent. Please note that actual tabstops are not supported.</param>
        /// <param name="wrappedLines">The number of added line breaks.</param>
        /// <returns>An array of one or more lines.</returns>
        public static string[] WrapAndMeasureValue(object value, ColumnFormat format, int columnWidth, int tabLength, out int wrappedLines)
        {
            var words = WordSplitter.Split(value.ToString(), tabLength);
            return WrapAndMeasureWords(words, format, columnWidth, out wrappedLines);
        }

        /// <summary>
        /// Given a value for a column, wrap the value into as many lines as required and calculate the number of line breaks
        /// that need to be added.
        /// </summary>
        /// <param name="words">The words to be displayed in the column.</param>
        /// <param name="format">The column's format specification.</param>
        /// <param name="columnWidth">The width allowed for the column.</param>
        /// <param name="wrappedLines">The number of added line breaks.</param>
        /// <returns>An array of one or more lines.</returns>
        public static string[] WrapAndMeasureWords(IEnumerable<SplitWord> words, ColumnFormat format, int columnWidth, out int wrappedLines)
        {
            wrappedLines = 0;
            var lines = new List<string>();
            var position = 0;
            var line = string.Empty;
            SplitWord lastWord = null;
            int spacesAdded;
            foreach (var splitWord in words.SelectMany(w => BreakWord(w, columnWidth)))
            {
                if (position + splitWord.Length + (lastWord == null ? 0 : Math.Min(lastWord.Length, lastWord.TrailingSpaces)) > columnWidth)
                {
                    if (lastWord != null)
                        line += lastWord.GetTrailingSpaces(0, out spacesAdded);

                    lines.Add(line);
                    wrappedLines++;
                    line = splitWord.GetWordValue();
                    lastWord = splitWord;
                    position = splitWord.Length;
                }
                else
                {
                    if (splitWord.TerminatesLine())
                    {
                        var wordValue = splitWord.GetWordValue();
                        var newLinePos = wordValue.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                        string newLineStart;
                        if (newLinePos >= 0)
                        {
                            var start = wordValue.Substring(0, newLinePos);
                            newLineStart = wordValue.Substring(newLinePos + Environment.NewLine.Length);
                            line += start;
                        }
                        else
                        {
                            if (lastWord != null)
                                line += lastWord.GetTrailingSpaces(0, out spacesAdded);
                            line += wordValue;
                            newLineStart = string.Empty;
                        }

                        lines.Add(line);
                        line = newLineStart;
                        position = 0;
                        lastWord = null;
                    }
                    else
                    {
                        position += splitWord.Length;
                        if (lastWord != null)
                        {
                            line += lastWord.GetTrailingSpaces(columnWidth - position, out spacesAdded);
                            position += spacesAdded;
                        }

                        line += splitWord.GetWordValue();
                        lastWord = splitWord;
                    }
                    
                }
            }

            if (position > 0)
            {
                if (lastWord != null)
                    line += lastWord.GetTrailingSpaces(0, out spacesAdded);
                lines.Add(line);
            }

            if (!lines.Any())
                lines.Add(string.Empty);

            if (format.ActualWidth == 0) return lines.ToArray();

            return lines.Select(l => new {Line = l, Width = format.ActualWidth + (l.Length - ColourString.Length(l))})
                .Select(l => ExpandLine(format, l.Line, l.Width))
                .ToArray();
        }

        private static string ExpandLine(ColumnFormat format, string line, int width)
        {
            var lineWidth = format.Alignment == ColumnAlign.Left ? -width : width;
            var formatSpec = "{0," + lineWidth.ToString(CultureInfo.InvariantCulture) + "}";
            return string.Format(formatSpec, line);
        }

        /// <summary>
        /// Break long words into chunks of one row width in length.
        /// 
        /// <see cref="BreakWord"/> guarantees that there will never be a word that 
        /// won't fit on a line, which simplifies <see cref="WrapValue"/>. 
        /// </summary>
        /// <param name="splitWord">The word to be split.</param>
        /// <param name="columnWidth">The width of the column.</param>
        /// <returns>An enumerable set of word chunks, one <see cref="columnWidth"/> or less wide.</returns>
        private static IEnumerable<SplitWord> BreakWord(SplitWord splitWord, int columnWidth)
        {
            while (splitWord.Length > columnWidth)
            {
                var newWord = new SplitWord(columnWidth, 0, ColourString.Substring(splitWord.WordValue, 0, columnWidth));
                newWord.AddPrefixInstructions(splitWord.PrefixInstructions);
                yield return newWord;

                var endWord = new SplitWord(splitWord.Length - columnWidth, splitWord.TrailingSpaces, ColourString.Substring(splitWord.WordValue, columnWidth));
                endWord.AddSuffixInstructions(splitWord.SuffixInstructions);

                splitWord = endWord;
            }

            yield return splitWord;
        }
    }
}