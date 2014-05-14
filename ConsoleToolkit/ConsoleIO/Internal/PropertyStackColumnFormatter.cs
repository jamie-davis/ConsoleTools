using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class PropertyStackColumnFormatter
    {
        public static IEnumerable<string> Format(IEnumerable<PropertyColumnFormat> stackedColumns, object item, int width, int tabLength = 4)
        {
            return stackedColumns.SelectMany(c => FormatCol(c, item, width, tabLength));
        }

        public static IEnumerable<string> Format(IEnumerable<PropertyColumnFormat> stackedColumns, IEnumerable<string> preFormattedValues, int width, int tabLength = 4)
        {
            var values = preFormattedValues.ToList();
            return stackedColumns.SelectMany((c, i) => FormatColWithPreFormattedValue(c, values[i], width, tabLength));
        }

        private static IEnumerable<string> FormatCol(PropertyColumnFormat pcf, object item, int width, int tabLength)
        {
            var rawValue = pcf.Property.GetValue(item);
            var value = ValueFormatter.Format(pcf.Format, rawValue);
            return FormatColWithPreFormattedValue(pcf, value, width, tabLength);
        }

        private static IEnumerable<string> FormatColWithPreFormattedValue(PropertyColumnFormat pcf, string preFormattedValue, int width, int tabLength)
        {
            return PropertyStackFormatter.Format(pcf.Format, preFormattedValue, width, tabLength);
        }
    }
}