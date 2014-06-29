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
        }

        /// <summary>
        /// Answers the question: how wide does the column have to be to format every value with no more than <see cref="maxLineBreaks"/>?
        /// </summary>
        /// <param name="maxLineBreaks">The maximum desired number of line breaks. Zero means "no line breaks allowed".</param>
        /// <returns>The number of characters the column needs to be to limit the line breaks to the specified count.</returns>
        public int MinWidth(int maxLineBreaks)
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

        private string FormatValue(object v)
        {
            return ValueFormatter.Format(_format, v);
        }

        private int FitToLines(FormattingIntermediate v, int maxLineBreaks)
        {
            for (var i = 1; i <= v.Width; i++)
            {
                var breaks = v.RenderableValue != null 
                    ? v.RenderableValue.CountWordWrapLineBreaks(_format, i)
                    : ColumnWrapper.CountWordwrapLineBreaks(v, _format, i);
                if (breaks <= maxLineBreaks) return i;
            }

            return v.Width;
        }

        public FormattingIntermediate GetSizeValue(int row)
        {
            Debug.Assert(_values.Count > row);
            return _values[row];
        }

        public int GetMaxLineBreaks(int width)
        {
            return _values.Max(v => ColumnWrapper.CountWordwrapLineBreaks(v, _format, width));
        }
    }
}