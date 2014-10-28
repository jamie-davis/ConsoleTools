using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class ColumnSizer
    {
        private readonly Type _columnType;
        private readonly int _tabLength;
        private List<FormattingIntermediate> _values = new List<FormattingIntermediate>();
        private ColumnFormat _format;
        private int _idealMinWidth;
        private bool _idealMinWidthValid;
        private Dictionary<int, int> _cachedMaxLineBreaks;

        public ColumnSizer(Type columnType, ColumnFormat format = null, int tabLength = 4)
        {
            _columnType = columnType;
            _tabLength = tabLength;
            _format = format ?? new ColumnFormat(columnType.Name, columnType);
        }

        public IEnumerable<FormattingIntermediate> Values { get { return _values; } }

        public void ColumnValue(object value)
        {
            if (value is IConsoleRenderer)
                _values.Add(new FormattingIntermediate(value as IConsoleRenderer));
            else
                _values.Add(FormatValue(value));
            _idealMinWidthValid = false;
            _cachedMaxLineBreaks = null;
        }

        /// <summary>
        /// Answers the question: how wide does the column have to be to format every value with no more than <see cref="maxLineBreaks"/>?
        /// <para/>
        /// This method needs to take the width restrictions into account, so for some columns, it may be that the width returned actually
        /// results in a different number of linebreaks than expected.
        /// </summary>
        /// <param name="maxLineBreaks">The maximum desired number of line breaks. Zero means "no line breaks allowed".</param>
        /// <returns>The number of characters the column needs to be to limit the line breaks to the specified count.</returns>
        public int MinWidth(int maxLineBreaks)
        {
            var minWidth = GetFixedMinWidth();
            if (minWidth > 0)
                return minWidth;

            return Math.Max(ComputeMinWidthFromData(maxLineBreaks), _format.MinWidth);
        }

        private int ComputeMinWidthFromData(int maxLineBreaks)
        {
            if (_columnType == typeof (string))
            {
                return _values.Max(v => FitToLines(v, maxLineBreaks));
            }
            return _values.Max(v => v.Width);
        }

        /// <summary>
        /// Returns the width the column needs to be to accomodate the longest word seen so far.
        /// </summary>
        /// <returns>The calculated width.</returns>
        public int GetIdealMinimumWidth()
        {
            if (_idealMinWidthValid) return _idealMinWidth;

            return CalculateIdealMinWidth();
        }

        private int CalculateIdealMinWidth()
        {
            var fixedWidth = GetFixedMinWidth();
            if (fixedWidth > 0)
            {
                _idealMinWidth = fixedWidth;
                _idealMinWidthValid = true;
                return _idealMinWidth;
            }

            if (_columnType == typeof(string))
            {
                _idealMinWidth = _values.Max(v => v.GetLongestWordLength(_tabLength));
                _idealMinWidthValid = true;
                return _idealMinWidth;
            }

            _idealMinWidth = _values.Max(v => v.Width);
            _idealMinWidthValid = true;
            return _idealMinWidth;

        }

        private int GetFixedMinWidth()
        {
            var fixedWidth = _format.FixedWidth > 0 ? _format.FixedWidth : _format.MinWidth;
            return fixedWidth;
        }

        private string FormatValue(object v)
        {
            return ValueFormatter.Format(_format, v);
        }

        private int FitToLines(FormattingIntermediate v, int maxLineBreaks)
        {
            var width = v.Width;
            if (width == 0) return 1;

            var tooWide = 0;
            var tooNarrow = 0;
            do
            {
                var breaks = v.RenderableValue != null
                                 ? v.RenderableValue.CountWordWrapLineBreaks(width)
                                 : ColumnWrapper.CountWordwrapLineBreaks(v, _format, width);
                if (breaks <= maxLineBreaks)
                    tooWide = width;
                else
                    tooNarrow = width;

                if (tooWide - tooNarrow == 1) return tooWide;

                if (tooWide == 0)
                    width *= 2;
                else
                    width = tooNarrow + (tooWide - tooNarrow) / 2;
            } while (true);
        }

        public FormattingIntermediate GetSizeValue(int row)
        {
            Debug.Assert(_values.Count > row);
            return _values[row];
        }

        public int GetMaxLineBreaks(int width)
        {
            if (_cachedMaxLineBreaks == null)
                _cachedMaxLineBreaks = new Dictionary<int, int>();
            else if (_cachedMaxLineBreaks.ContainsKey(width))
                return _cachedMaxLineBreaks[width];

            var maxLineBreaks = _values.Max(v => ColumnWrapper.CountWordwrapLineBreaks(v, _format, width));
            _cachedMaxLineBreaks[width] = maxLineBreaks;
            return maxLineBreaks;
        }
    }
}