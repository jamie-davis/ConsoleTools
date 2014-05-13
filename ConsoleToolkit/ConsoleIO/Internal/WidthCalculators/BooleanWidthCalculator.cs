using System;

namespace ConsoleToolkit.ConsoleIO.Internal.WidthCalculators
{
    /// <summary>
    /// Calculate the defaults for Boolean column widths.
    /// </summary>
    public static class BooleanWidthCalculator
    {
        public static int Max(ColumnFormat format)
        {
            return Math.Max(ValueFormatter.Format(format, true).Length,
                ValueFormatter.Format(format, false).Length);
        }

        public static int Min(ColumnFormat format)
        {
            return Math.Min(ValueFormatter.Format(format, true).Length,
                ValueFormatter.Format(format, false).Length);
        }
    }
}