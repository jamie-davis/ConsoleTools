using System;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class ColumnShrinker
    {
        public static void ShrinkColumns(int width, int seperatorOverhead, ColumnSizingParameters parameters)
        {
            var minPossibleWidth = MinPossibleWidth(seperatorOverhead, parameters);
            if (minPossibleWidth > width)
                StackColumns(width, parameters);
            else
            {
                FitColumns(width, seperatorOverhead, parameters);
            }
        }

        private static int MinPossibleWidth(int seperatorOverhead, ColumnSizingParameters parameters)
        {
            var minStackWidth = parameters.StackSizer != null ? parameters.StackSizer.GetMinWidth(parameters.TabLength) : 0;
            var minPossibleWidth = parameters.Sizers.Sum(s => s.Sizer.GetIdealMinimumWidth()) + seperatorOverhead + minStackWidth;
            return minPossibleWidth;
        }

        private static void StackColumns(int width, ColumnSizingParameters parameters)
        {
            //the columns do not fit, so start stacking until they do.
            parameters.StackSizer = new PropertyStackColumnSizer();

            ColumnWidthNegotiator.ColumnSizerInfo lastColumn;
            while ((lastColumn = parameters.Sizers.LastOrDefault()) != null)
            {
                parameters.StackSizer.AddColumn(lastColumn.PropertyColumnFormat, lastColumn.Sizer.Values);
                parameters.Sizers.Remove(lastColumn);
                parameters.Columns.Remove(lastColumn.PropertyColumnFormat);

                var seperatorOverhead = parameters.Sizers.Count * parameters.SeperatorLength; //all columns in sizer list need seperator.
                if (MinPossibleWidth(seperatorOverhead, parameters) <= width)
                    break;
            }

            if (parameters.Sizers.Any())
            {
                FitColumns(width, parameters.Sizers.Count * parameters.SeperatorLength, parameters);
                parameters.StackedColumnWidth = parameters.StackSizer.GetMinWidth(parameters.TabLength);
            }
            else
                parameters.StackedColumnWidth = width;
        }

        private static void FitColumns(int width, int seperatorOverhead, ColumnSizingParameters parameters)
        {
            if (!parameters.Sizers.Any()) return;

            var minStackWidth = (parameters.StackSizer == null ? 0 : parameters.StackSizer.GetMinWidth(parameters.TabLength));
            var linebreaks = 0;
            do
            {
                var widths = parameters.Sizers
                    .Select(s => new { Sizer = s, Width = Math.Max(s.Sizer.MinWidth(linebreaks), s.Sizer.GetIdealMinimumWidth()) })
                    .ToList();
                var totalWidth = widths.Sum(w => w.Width);
                if (totalWidth <= width - seperatorOverhead - minStackWidth)
                {
                    foreach (var propWidth in widths)
                    {
                        propWidth.Sizer.PropertyColumnFormat.Format.SetActualWidth(propWidth.Width);
                    }
                    break;
                }

                linebreaks++;
            } while (true);
        }

    }
}