using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    public static class TabularReport
    {
        /// <summary>
        /// Format a collection of rows as a tabular report.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="data">The enumerable items.</param>
        /// <param name="columns">Column formatting information. If this is not provided, default column formats will be used.</param>
        /// <param name="width">The width that the report is allowed to occupy.</param>
        /// <param name="numRowsToUseForSizing">The number of rows that should be used for automatically sizing columns.</param>
        /// <returns>The formatted report lines.</returns>
        public static IEnumerable<string> Format<T>(IEnumerable<T> data, IEnumerable<ColumnFormat> columns, int width, int numRowsToUseForSizing = 0)
        {
            var output = new BlockingCollection<string>();

            Task.Factory.StartNew(() => DoFormat(output, data, columns, width, numRowsToUseForSizing));

            return output.GetConsumingEnumerable();
        }

        private static void DoFormat<T>(BlockingCollection<string> output, IEnumerable<T> data, IEnumerable<ColumnFormat> specifiedColumns, int width, int numRowsToUseForSizing, int tabLength = 4)
        {
            try
            {
                var columns = FormatAnalyser.Analyse(typeof (T), specifiedColumns);
                var sizer = new ColumnWidthNegotiator(columns, 1);
                var sizeData = new List<T>();

                sizer.AddHeadings();

                var sizeRows = 0;
                var dataEnumerator = data.GetEnumerator();
                bool enumeratorValid;
                while ((enumeratorValid = dataEnumerator.MoveNext())
                       && (numRowsToUseForSizing == 0 
                           || sizeRows++ < numRowsToUseForSizing))
                {
                    var item = dataEnumerator.Current;
                    sizer.AddRow(item);
                    sizeData.Add(item);
                }

                sizer.CalculateWidths(width);

                const string columnSeperator = " ";
                var widths = columns.Select(c => c.Format.ActualWidth)
                    .Concat(StackedColumnWidths(sizer))
                    .ToArray();

                ReportHeadings(sizer.Columns, output, tabLength, columnSeperator);
                ReportRowsUsedForSizing(columns, sizer, output, tabLength, columnSeperator, widths);

                if (enumeratorValid)
                {
                    do
                    {
                        ReportRow(dataEnumerator.Current, columns, sizer, output, tabLength, columnSeperator, widths);
                    } while (dataEnumerator.MoveNext());

                }
            }
            finally
            {
                output.CompleteAdding();
            }
        }

        private static void ReportRow(object item, List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, BlockingCollection<string> output, int tabLength, string columnSeperator, int[] widths)
        {
            var rowValues = columns
                .Select(c => ValueFormatter.Format(c.Format, c.Property.GetValue(item)))
                .ToList();
            ReportRowValues(columns, sizer, output, tabLength, columnSeperator, rowValues, widths);
        }

        private static void ReportRowsUsedForSizing(List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, BlockingCollection<string> output, int tabLength, string columnSeperator, int[] widths)
        {
            foreach (var rowValues in sizer.GetSizingValues().Select(v => v.ToList()))
            {
                ReportRowValues(columns, sizer, output, tabLength, columnSeperator, rowValues, widths);
            }
        }

        private static void ReportRowValues(List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, BlockingCollection<string> output, int tabLength,
            string columnSeperator, List<string> rowValues, int[] widths)
        {
            var wrappedValues = columns
                .Select((c, i) => ColumnWrapper.WrapValue(rowValues[i], c.Format, c.Format.ActualWidth, tabLength))
                .Concat(StackedColumnValues(rowValues, sizer, tabLength))
                .Select(ColourSeal.Seal)
                .ToArray();
            output.Add(ReportColumnAligner.AlignColumns(widths, wrappedValues, ColVerticalAligment.Top, columnSeperator));
        }

        private static IEnumerable<string[]> StackedColumnValues(IEnumerable<string> rowValues, ColumnWidthNegotiator sizer, int tabLength)
        {
            if (!sizer.StackedColumns.Any())
                return new string[][] { };

            var formattedLines = PropertyStackColumnFormatter.Format(sizer.StackedColumns, rowValues.Skip(sizer.Columns.Count),
                sizer.StackedColumnWidth, tabLength)
                .ToArray();
            return new[] {formattedLines};
        }

        private static IEnumerable<int> StackedColumnWidths(ColumnWidthNegotiator sizer)
        {
            if (!sizer.StackedColumns.Any())
                return new int[] { };

            return new[] {sizer.StackedColumnWidth};
        }

        private static void ReportHeadings(List<PropertyColumnFormat> columns, BlockingCollection<string> output, int tabLength, string columnSeperator)
        {
            if (!columns.Any()) return;

            var widths = columns.Select(c => c.Format.ActualWidth).ToArray();
            var headings = columns.Select(c => WrapValue(tabLength, c, c.Format.Heading)).ToArray();
            var underlines = columns.Select(c => new [] {new string('-', c.Format.ActualWidth)}).ToArray();
            var headingLines = ReportColumnAligner.AlignColumns(widths, headings, ColVerticalAligment.Bottom, columnSeperator);
            var headingUnderLines = ReportColumnAligner.AlignColumns(widths, underlines, ColVerticalAligment.Bottom, columnSeperator);

            output.Add(headingLines);
            output.Add(headingUnderLines);
       }

        private static string[] WrapValue(int tabLength, PropertyColumnFormat pcf, object value)
        {
            var formatted = ValueFormatter.Format(pcf.Format, value);
            return ColumnWrapper.WrapValue(formatted, pcf.Format, pcf.Format.ActualWidth, tabLength);
        }
    }
}