using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Stretch columns in a column sizer list to fill the available width.
    /// </summary>
    internal static class ColumnExpander
    {
        /// <summary>
        /// Perform the stretch operation. This method takes any stacked columns into account.
        /// </summary>
        /// <param name="width">The available width.</param>
        /// <param name="seperatorOverhead">The amount of space occupied by column seperators.</param>
        /// <param name="parameters">The column sizing parameters for this operation.</param>
        /// <param name="maximiseWidth">True to systematically use all of the available width when sizing the columns.</param>
        public static void FillAvailableSpace(int width, int seperatorOverhead, ColumnSizingParameters parameters, bool maximiseWidth)
        {
            var columnPriorityList = parameters.Sizers.ToList();
            while (CurrentWidth(seperatorOverhead, parameters.Sizers, parameters.StackedColumnWidth) < width)
            {
                if (!WidenBasedOnLineBreaks(columnPriorityList) 
                    && !WidenBasedOnMaximumWidth(columnPriorityList, maximiseWidth) 
                    && (!maximiseWidth || !WidenUnrestrictedColumns(columnPriorityList)))
                    break;
            }
        }

        private static bool WidenUnrestrictedColumns(List<ColumnWidthNegotiator.ColumnSizerInfo> columnPriorityList)
        {
            var firstUnrestrictedColumn = columnPriorityList
                .FirstOrDefault(c => DefaultWidthCalculator.Max(c.PropertyColumnFormat.Format) == 0);

            if (firstUnrestrictedColumn == null)
                return false;
            WidenColumn(columnPriorityList, firstUnrestrictedColumn);
            return true;
        }

        private static bool WidenBasedOnMaximumWidth(List<ColumnWidthNegotiator.ColumnSizerInfo> columnPriorityList, bool maximiseWidth)
        {
            var proportionOfMaxWidth = columnPriorityList
                .Select(
                    s =>
                        new
                        {
                            Sizer = s,
                            Max = GetMaxColumnWidth(s, maximiseWidth),
                            Actual = s.PropertyColumnFormat.Format.ActualWidth
                        })
                .Where(c => c.Max > c.Actual)
                .Select(c => new {c.Sizer, Proportion = (double)c.Actual/c.Max})
                .OrderByDescending(c => c.Proportion)
                .ToList();

            if (!proportionOfMaxWidth.Any()) return false;

            var targetProp = proportionOfMaxWidth.First().Proportion;
// ReSharper disable once CompareOfFloatsByEqualityOperator
            var matches = proportionOfMaxWidth.Where(c => c.Proportion == targetProp).ToList();
            var column = columnPriorityList.FirstOrDefault(c => matches.Any(m => ReferenceEquals(m.Sizer.Sizer, c.Sizer)));
            WidenColumn(columnPriorityList, column);
            return true;
        }

        private static int GetMaxColumnWidth(ColumnWidthNegotiator.ColumnSizerInfo s, bool includeHeadingWidth)
        {
            var columnFormat = s.PropertyColumnFormat.Format;
            var headingLength = includeHeadingWidth ? columnFormat.Heading.Length : 0;
            return Math.Max(headingLength, DefaultWidthCalculator.Max(columnFormat));
        }

        private static bool WidenBasedOnLineBreaks(List<ColumnWidthNegotiator.ColumnSizerInfo> columnPriorityList)
        {
            var lineBreaks = columnPriorityList
                .Select(s => new {Sizer = s, MaxBreaks = s.Sizer.GetMaxLineBreaks(s.PropertyColumnFormat.Format.ActualWidth)})
                .Where(l => l.MaxBreaks > 0)
                .OrderByDescending(l => l.MaxBreaks)
                .ToList();

            if (!lineBreaks.Any())
                return false;

            var targetLineBreaks = lineBreaks.First().MaxBreaks;
            var matches = lineBreaks.Where(l => l.MaxBreaks == targetLineBreaks).ToList();
            var column = columnPriorityList.FirstOrDefault(c => matches.Any(m => ReferenceEquals(m.Sizer.Sizer, c.Sizer)));
            if (column == null)
                return false;

            WidenColumn(columnPriorityList, column);
            return true;
        }

        private static void WidenColumn(List<ColumnWidthNegotiator.ColumnSizerInfo> columnPriorityList, ColumnWidthNegotiator.ColumnSizerInfo column)
        {
            var format = column.PropertyColumnFormat.Format;
            format.SetActualWidth(format.ActualWidth + 1);
            columnPriorityList.Remove(column);
            columnPriorityList.Add(column);
        }

        private static int CurrentWidth(int seperatorOverhead, IEnumerable<ColumnWidthNegotiator.ColumnSizerInfo> sizers, int stackedColumnWidth)
        {
            return sizers.Sum(s => s.PropertyColumnFormat.Format.ActualWidth) + seperatorOverhead + stackedColumnWidth;
        }

    }
}