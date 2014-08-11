using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class PropertyStackColumnFormatter
    {
        public static IEnumerable<string> Format(IEnumerable<PropertyColumnFormat> stackedColumns, object item, 
            int width, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            return stackedColumns.SelectMany((c, i) => FormatCol(c, item, width, tabLength, i == 0 ? firstLineHangingIndent : 0));
        }

        public static IEnumerable<string> Format(IEnumerable<PropertyColumnFormat> stackedColumns, IEnumerable<string> preFormattedValues, 
            int width, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            var values = preFormattedValues.ToList();
            return stackedColumns.SelectMany((c, i) => FormatColWithPreFormattedValue(c, values[i], width, tabLength, i == 0 ? firstLineHangingIndent : 0));
        }

        public static IEnumerable<string> Format(IEnumerable<PropertyColumnFormat> stackedColumns, 
            IEnumerable<FormattingIntermediate> preFormattedValues, int width, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            var values = preFormattedValues.ToList();
            return stackedColumns.SelectMany((c, i) => FormatColWithIntermediate(c, values[i], width, tabLength, i == 0 ? firstLineHangingIndent : 0));
        }

        private static IEnumerable<string> FormatCol(PropertyColumnFormat pcf, object item, 
            int width, int tabLength, int firstLineHangingIndent)
        {
            var rawValue = pcf.Property.GetValue(item);
            var value = ValueFormatter.Format(pcf.Format, rawValue);
            return FormatColWithPreFormattedValue(pcf, value, width, tabLength, firstLineHangingIndent);
        }

        private static IEnumerable<string> FormatColWithPreFormattedValue(PropertyColumnFormat pcf, string preFormattedValue, 
            int width, int tabLength, int firstLineHangingIndent)
        {
            return PropertyStackFormatter.Format(pcf.Format, preFormattedValue, width, tabLength, firstLineHangingIndent);
        }

        private static IEnumerable<string> FormatColWithIntermediate(PropertyColumnFormat pcf, FormattingIntermediate preFormattedValue, 
            int width, int tabLength, int firstLineHangingIndent)
        {
            return PropertyStackFormatter.Format(pcf.Format, FormatIntermediateValue(preFormattedValue, width), width, tabLength, firstLineHangingIndent);
        }

        private static object FormatIntermediateValue(FormattingIntermediate preFormattedValue, int width)
        {
            if (preFormattedValue.TextValue != null)
                return preFormattedValue.TextValue;

            return preFormattedValue.RenderableValue;
        }
    }
}