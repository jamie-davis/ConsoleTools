using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class MinReportWidthCalculator
    {
        public static int Calculate<T>(IEnumerable<T> rep, int seperatorLength, int tabLength = 4)
        {
            return PerformCalculation<T>(tabLength, seperatorLength, sizers =>
            {
                foreach (var row in rep)
                {
                    foreach (var sizer in sizers)
                    {
                        var value = sizer.PropertyColumnFormat.Property.GetValue(row, null);
                        sizer.Sizer.ColumnValue(value);
                    }
                }

            });
        }

        public static int Calculate<T>(CachedRows<T> rep, int seperatorLength, int tabLength = 4)
        {
            return PerformCalculation<T>(tabLength, seperatorLength, sizers =>
            {
                foreach (var row in rep.GetRows())
                {
                    foreach (var sizer in sizers)
                    {
                        var value = row.Columns.FirstOrDefault(c => c.Property == sizer.PropertyColumnFormat.Property);
                        if (value != null && value.Value != null)
                            sizer.Sizer.ColumnValue(value.Value);
                    }
                }
            });
        }

        private static int PerformCalculation<T>(int tabLength, int seperatorLength, Action<List<ColumnWidthNegotiator.ColumnSizerInfo>> applyRows)
        {
            var columns = FormatAnalyser.Analyse(typeof(T), null, true);
            var sizers = columns
                .Select(c => new ColumnWidthNegotiator.ColumnSizerInfo(c, tabLength))
                .ToList();

            applyRows(sizers);

            foreach (var columnSizerInfo in sizers)
            {
                columnSizerInfo.Sizer.ColumnValue(columnSizerInfo.PropertyColumnFormat.Format.Heading);
            }

            return sizers.Sum(s => s.Sizer.GetIdealMinimumWidth())
                   + ((sizers.Count - 1) * seperatorLength);
        }
    }
}