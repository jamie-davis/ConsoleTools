using System;

namespace ConsoleToolkit.ConsoleIO.Internal.WidthCalculators
{
    /// <summary>
    /// Calculate defaults for numeric column widths.
    /// </summary>
    public static class NumericWidthCalculator
    {
        public static int Min(ColumnFormat format)
        {
            return GetMaxWidth(format);
        }

        public static int Max(ColumnFormat format)
        {
            return GetMaxWidth(format);
        }

        private static int GetMaxWidth(ColumnFormat format)
        {
            object maxVal;
            object minVal;

            if (format.Type == typeof (double))
            {
                maxVal = 999999999999d;
                minVal = -999999999999d;
            }
            else if (format.Type == typeof (float))
            {
                maxVal = 999999999f;
                minVal = -999999999f;
            }
            else
            {
                var maxValMember = format.Type.GetField("MaxValue");
                if (maxValMember == null)
                    maxVal = int.MaxValue;
                else
                    maxVal = maxValMember.GetValue(null);

                var minValMember = format.Type.GetField("MinValue");
                if (minValMember == null)
                    minVal = int.MinValue;
                else
                    minVal = minValMember.GetValue(null);
                
            }
            return Math.Max(ValueFormatter.Format(format, maxVal).Length, ValueFormatter.Format(format, minVal).Length);
       }
    }
}
