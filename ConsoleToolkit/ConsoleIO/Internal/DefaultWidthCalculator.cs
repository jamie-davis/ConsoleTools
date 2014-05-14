using System;
using System.Collections.Generic;
using ConsoleToolkit.ConsoleIO.Internal.WidthCalculators;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Determines the ideal width for a column based purely on the format.
    /// 
    /// This is the place for any logic that helps to make a sensible decision about how wide a column needs to be. For example, this could be 
    /// determined more accurately by examining the <see cref="ColumnFormat.FormatTemplate"/>.
    /// </summary>
    internal static class DefaultWidthCalculator
    {
        /// <summary>
        /// Value to be used as the minimum width if all else fails.
        /// </summary>
        private const int DefaultMinWidth = 6;

        /// <summary>
        /// Value to be used as the maximum width if all else fails.
        /// </summary>
        private const int DefaultMaxWidth = 12;

        private static readonly Dictionary<Type, Func<ColumnFormat, int>> MinWidthHandlers = new Dictionary
            <Type, Func<ColumnFormat, int>>
        {
            {typeof (sbyte), NumericWidthCalculator.Min},
            {typeof (byte), NumericWidthCalculator.Min},
            {typeof (short), NumericWidthCalculator.Min},
            {typeof (ushort), NumericWidthCalculator.Min},
            {typeof (int), NumericWidthCalculator.Min},
            {typeof (uint), NumericWidthCalculator.Min},
            {typeof (long), NumericWidthCalculator.Min},
            {typeof (ulong), NumericWidthCalculator.Min},
            {typeof (decimal), NumericWidthCalculator.Min},
            {typeof (float), NumericWidthCalculator.Min},
            {typeof (double), NumericWidthCalculator.Min},
            {typeof (string), StringWidthCalculator.Min},
            {typeof (char), f => 1},
            {typeof (DateTime), DateTimeWidthCalculator.Calculate},
            {typeof (TimeSpan), TimeSpanWidthCalculator.Calculate},
            {typeof (bool), BooleanWidthCalculator.Max},
        };

        private static readonly Dictionary<Type, Func<ColumnFormat, int>> MaxWidthHandlers = new Dictionary<Type, Func<ColumnFormat, int>>
        {
            {typeof (sbyte), NumericWidthCalculator.Max},
            {typeof (byte), NumericWidthCalculator.Max},
            {typeof (short), NumericWidthCalculator.Max},
            {typeof (ushort), NumericWidthCalculator.Max},
            {typeof (int), NumericWidthCalculator.Max},
            {typeof (uint), NumericWidthCalculator.Max},
            {typeof (long), NumericWidthCalculator.Max},
            {typeof (ulong), NumericWidthCalculator.Max},
            {typeof (decimal), NumericWidthCalculator.Max},
            {typeof (float), NumericWidthCalculator.Max},
            {typeof (double), NumericWidthCalculator.Max},
            {typeof (string), f => 0},
            {typeof (char), f => 1},
            {typeof (DateTime), DateTimeWidthCalculator.Calculate},
            {typeof (TimeSpan), TimeSpanWidthCalculator.Calculate},
            {typeof (bool), BooleanWidthCalculator.Min},
        };

        public static int Min(ColumnFormat format)
        {
            Func<ColumnFormat, int> func;
            if (MinWidthHandlers.TryGetValue(format.Type, out func))
                return func(format);

            return DefaultMinWidth;
        }

        public static int Max(ColumnFormat format)
        {
            Func<ColumnFormat, int> func;
            if (MaxWidthHandlers.TryGetValue(format.Type, out func))
                return func(format);
            return DefaultMaxWidth;
        }
    }
}