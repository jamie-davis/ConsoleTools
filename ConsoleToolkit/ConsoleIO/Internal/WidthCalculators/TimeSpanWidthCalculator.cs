using System;

namespace ConsoleToolkit.ConsoleIO.Internal.WidthCalculators
{
    /// <summary>
    /// Calculate the defaults for TimeSpan column widths.
    /// </summary>
    public static class TimeSpanWidthCalculator
    {
        public static int Calculate(ColumnFormat format)
        {
            return ValueFormatter.Format(format, TimeSpan.MinValue).Length;
        }
    }
}