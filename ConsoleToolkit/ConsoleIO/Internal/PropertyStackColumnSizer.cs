using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class PropertyStackColumnSizer
    {
        class StackPropertyInfo
        {
            public PropertyColumnFormat Column;
            public List<string> Values;

            public int GetLongestWordLength(int tabLength)
            {
                if (!Values.Any()) return 0;

                return Values.Max(v => WordSplitter.Split(v, tabLength).Max(w => w.Length));
            }

            public int GetFirstLineLength(int tabLength)
            {
                var headingOverhead = Column.Format.Heading.Length + 2;
                if (!Values.Any()) return headingOverhead;

                return Values.Max(v =>
                {
                    var word = WordSplitter.Split(v, tabLength).FirstOrDefault();
                    return headingOverhead + (word == null ? 0 : word.Length);
                });
            }
        }

        private List<StackPropertyInfo> _columns = new List<StackPropertyInfo>(); 
        
        public IEnumerable<PropertyColumnFormat> Columns { get { return _columns.Select(c => c.Column); } }

        public void AddColumn(PropertyColumnFormat format, IEnumerable<string> values)
        {
            var newItem = new StackPropertyInfo()
            {
                Column = format,
                Values = values.ToList()
            };
            _columns.Insert(0, newItem);
        }

        public int GetMinWidth(int tabLength = 4)
        {
            if (!_columns.Any()) return 0;

            var longestWord = _columns.Max(c => c.GetLongestWordLength(tabLength));
            var longestFirstLine = _columns.Max(c => c.GetFirstLineLength(tabLength));

            return Math.Max(longestFirstLine, longestWord);
        }

        public IEnumerable<string> GetSizeValues(int row)
        {
            return _columns.Select(c => c.Values[row]);
        }
    }
}