using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

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
        /// <param name="firstLineHangingIndent">Number of characters at the start of the first line that cannot be used for 
        /// wrapping. This is required when the wrapped text begins part way along an existing line.</param>
        /// <returns>An array of one or more lines.</returns>
        public static string[] WrapValue(object value, ColumnFormat format, int columnWidth, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            Debug.Assert(columnWidth > 0);
            int wrappedLines;
            return WrapAndMeasureValue(value, format, columnWidth, tabLength, firstLineHangingIndent, out wrappedLines);
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
        /// <param name="firstLineHangingIndent">Number of characters at the start of the first line that cannot be used for 
        /// wrapping. This is required when the wrapped text begins part way along an existing line.</param>
        /// <returns>The number of added line breaks.</returns>
        public static int CountWordwrapLineBreaks(object value, ColumnFormat format, int columnWidth, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            int wrappedLines;
            WrapAndMeasureValue(value, format, columnWidth, tabLength, firstLineHangingIndent, out wrappedLines);
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
        /// <param name="firstLineHangingIndent">Number of characters at the start of the first line that cannot be used for 
        /// wrapping. This is required when the wrapped text begins part way along an existing line.</param>
        /// <param name="wrappedLines">The number of added line breaks.</param>
        /// <returns>An array of one or more lines.</returns>
        public static string[] WrapAndMeasureValue(object value, ColumnFormat format, int columnWidth, int tabLength, int firstLineHangingIndent, out int wrappedLines)
        {
            var intermediate = value as FormattingIntermediate;
            if (intermediate == null || intermediate.RenderableValue == null)
            {
                var words = WordSplitter.Split(GetValueString(value, columnWidth), tabLength);
                return WrapAndMeasureWords(words, format, columnWidth, firstLineHangingIndent, out wrappedLines);
                
            }
            
            return intermediate.RenderableValue.Render(columnWidth, out wrappedLines).ToArray();
        }

        private static string GetValueString(object value, int columnWidth)
        {
            if (value is FormattingIntermediate)
            {
                var intermediate = value as FormattingIntermediate;
                if (intermediate.RenderableValue == null)
                    return intermediate.TextValue;
                
                return intermediate.ToString(columnWidth);
            }
            return value.ToString();
        }

        /// <summary>
        /// Given a value for a column, wrap the value into as many lines as required and calculate the number of line breaks
        /// that need to be added.
        /// </summary>
        /// <param name="words">The words to be displayed in the column.</param>
        /// <param name="format">The column's format specification.</param>
        /// <param name="columnWidth">The width allowed for the column.</param>
        /// <param name="firstLineHangingIndent">Number of characters at the start of the first line that cannot be used for 
        /// wrapping. This is required when the wrapped text begins part way along an existing line.</param>
        /// <param name="wrappedLines">The number of added line breaks.</param>
        /// <returns>An array of one or more lines.</returns>
        public static string[] WrapAndMeasureWords(IEnumerable<SplitWord> words, ColumnFormat format, int columnWidth, int firstLineHangingIndent, out int wrappedLines)
        {
            wrappedLines = 0;
            var lines = new List<string>();
            var position = firstLineHangingIndent;
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
                    line += lastWord.GetTrailingSpaces(columnWidth - position, out spacesAdded);
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