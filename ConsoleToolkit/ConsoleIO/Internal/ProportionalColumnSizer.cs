using System;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class ProportionalColumnSizer
    {
        public static void Size(int width, int seperatorOverhead, ColumnSizingParameters parameters)
        {
            var nonProportionalColumnTotalWidth = parameters.Sizers.Where(s => !s.WidthIsProportional()).Sum(s => s.PropertyColumnFormat.Format.ActualWidth);
            var proportionalColumns = parameters.Sizers.Where(s => s.WidthIsProportional()).ToList();

            var availableWidth = width - seperatorOverhead - nonProportionalColumnTotalWidth - parameters.StackedColumnWidth;

            var totalProportions = proportionalColumns.Sum(s => s.PropertyColumnFormat.Format.ProportionalWidth);
            var proportions = proportionalColumns
                .Select(s => new { ProportionalWidthCalc = MakeProportionalWidth(availableWidth, totalProportions, s.PropertyColumnFormat.Format), s.PropertyColumnFormat.Format})
                .Select(s => new { ProportionalWidth = s.ProportionalWidthCalc.Item1, s.Format, ExcessWidth = s.ProportionalWidthCalc.Item2 })
                .OrderByDescending(s => s.Format.ProportionalWidth)
                .ToList();

            var availableCharacters = availableWidth - proportions.Sum(p => p.ProportionalWidth);

            var adjuster = availableCharacters < 0 ? -1 : 1;
            while (availableCharacters != 0)
            {
                var current = proportions.First(c => c.ProportionalWidth + adjuster > 1);
                proportions.Remove(current);
                if (current.ExcessWidth > 0 && adjuster > 0)
                    proportions.Add(new { current.ProportionalWidth, current.Format, ExcessWidth = current.ExcessWidth - adjuster});
                else if (current.ExcessWidth == 0 && (current.Format.MaxWidth == 0 || current.ProportionalWidth + adjuster <= current.Format.MaxWidth))
                {
                    proportions.Add(new { ProportionalWidth = current.ProportionalWidth + adjuster, current.Format, current.ExcessWidth});
                    availableCharacters -= adjuster;
                }
                else
                    proportions.Add(current);
            }

            foreach(var col in proportions)
                col.Format.SetActualWidth(col.ProportionalWidth);
        }

        private static Tuple<int, int> MakeProportionalWidth(int availableWidth, double totalProportions, ColumnFormat format)
        {
            var proportionalWidth = (availableWidth/totalProportions)*format.ProportionalWidth;
            var intWidth = (int)Math.Floor(proportionalWidth);
            var excessWidth = 0;

            if (intWidth < format.MinWidth)
            {
                excessWidth = format.MinWidth - intWidth;
                intWidth = format.MinWidth;
            }

            if (format.MaxWidth > 0  && intWidth > format.MaxWidth)
            {
                intWidth = format.MaxWidth;
                excessWidth = 0;
            }

            return Tuple.Create(intWidth == 0 ? 1 : intWidth, excessWidth);
        }
    }
}