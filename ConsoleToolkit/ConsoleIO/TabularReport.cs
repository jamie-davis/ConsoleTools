using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// A formatter for tabular data.
    /// </summary>
    public static class TabularReport
    {
        internal const string DefaultColumnDivider = " ";

        internal class Statistics
        {
            public int WordWrapLineBreaks { get; set; }
        }

        /// <summary>
        /// Format a collection of rows as a tabular report.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="data">The enumerable items.</param>
        /// <param name="columns">Column formatting information. If this is not provided, default column formats will be used.</param>
        /// <param name="width">The width that the report is allowed to occupy.</param>
        /// <param name="numRowsToUseForSizing">The number of rows that should be used for automatically sizing columns.</param>
        /// <param name="options">Options that control the formatting of the report.</param>
        /// <param name="columnDivider">A string that will be used to divide columns.</param>
        /// <returns>The formatted report lines.</returns>
        public static IEnumerable<string> Format<T>(IEnumerable<T> data, IEnumerable<ColumnFormat> columns, 
            int width, int numRowsToUseForSizing = 0, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnDivider = null)
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            using (new TempCulture(culture))
            {
                    var statistics = new Statistics();
                    foreach (var l in DoFormat(data, columns, width, numRowsToUseForSizing, 4, options, columnDivider, statistics))
                        yield return l;
            }
        }

        /// <summary>
        /// Format a collection of cached rows as a tabular report. The culture in force at the start of the enumeration will be used for all
        /// rows of the report. It is not possible to change the culture while the report is being enumerated.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="data">The cached row items.</param>
        /// <param name="columns">Column formatting information. If this is not provided, default column formats will be used.</param>
        /// <param name="width">The width that the report is allowed to occupy.</param>
        /// <param name="options">Options that effect formatting.</param>
        /// <param name="columnDivider">A string that will be used to divide the report columns.</param>
        /// <returns>The formatted report lines.</returns>
        public static IEnumerable<string> Format<T>(CachedRows<T> data, IEnumerable<ColumnFormat> columns, int width, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnDivider = null)
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            using (new TempCulture(culture))
            {
                var statistics = new Statistics();
                foreach (var l in DoFormatFromCachedRows(data, columns, width, 0, 4, options, columnDivider, statistics))
                    yield return l;
            }
        }

        /// <summary>
        /// Format a collection of cached rows as a tabular report. The culture in force at the start of the enumeration will be used for all
        /// rows of the report. It is not possible to change the culture while the report is being enumerated.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="data">The cached row items.</param>
        /// <param name="columns">Column formatting information. If this is not provided, default column formats will be used.</param>
        /// <param name="width">The width that the report is allowed to occupy.</param>
        /// <param name="wrappingLineBreaks">The number of linebreaks added due to wrapping.</param>
        /// <param name="options">Options that effect the formatting.</param>
        /// <param name="columnDivider">The string to be used to divide columns.</param>
        /// <returns>The formatted report lines.</returns>
        internal static IEnumerable<string> Format<T>(CachedRows<T> data, IEnumerable<ColumnFormat> columns, int width, Statistics wrappingLineBreaks, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnDivider = null)
        {
            var addedLineBreaks = 0;

            var statistics = new Statistics();
            foreach (var l in DoFormatFromCachedRows(data, columns, width, 0, 4, options, columnDivider, statistics))
                yield return l;
            addedLineBreaks = statistics.WordWrapLineBreaks;

            wrappingLineBreaks.WordWrapLineBreaks = addedLineBreaks;
        }

        private static IEnumerable<string> DoFormatFromCachedRows<T>(CachedRows<T> data, IEnumerable<ColumnFormat> specifiedColumns, int width, int numRowsToUseForSizing, int tabLength, ReportFormattingOptions options, string columnDivider, Statistics statistics)
        {
            var columns = FormatAnalyser.Analyse(typeof(T), specifiedColumns);
            return FormatFromDataFeed<CachedRow<T>>(data.GetRows(), width, numRowsToUseForSizing, tabLength, columns, (sz, item) => sz.AddRow(item), options, columnDivider, statistics);
        }

        private static IEnumerable<string> DoFormat<T>(IEnumerable<T> data, IEnumerable<ColumnFormat> specifiedColumns, int width, int numRowsToUseForSizing, int tabLength, ReportFormattingOptions options, string columnDivider, Statistics statistics)
        {
            var columns = FormatAnalyser.Analyse(typeof(T), specifiedColumns);
            return FormatFromDataFeed<T>(data, width, numRowsToUseForSizing, tabLength, columns, (sz, item) => sz.AddRow(item), options, columnDivider, statistics);
        }

        private static IEnumerable<string> FormatFromDataFeed<T>(IEnumerable data,
            int width, int numRowsToUseForSizing, int tabLength,
            List<PropertyColumnFormat> columns, Action<ColumnWidthNegotiator, T> addRowAction,
            ReportFormattingOptions options,
            string columnDivider,
            Statistics statistics)
        {
            if (columnDivider == null) columnDivider = DefaultColumnDivider;

            var sizer = new ColumnWidthNegotiator(columns, columnDivider.Length);

            var headingsRequired = (options & ReportFormattingOptions.OmitHeadings) == 0;
            if (headingsRequired)
                sizer.AddHeadings();

            var sizeRows = 0;
            var dataEnumerator = data.GetEnumerator();
            bool enumeratorValid;
            while ((enumeratorValid = dataEnumerator.MoveNext())
                   && (numRowsToUseForSizing == 0
                       || sizeRows++ < numRowsToUseForSizing))
            {
                var item = dataEnumerator.Current;
                addRowAction(sizer, (T) item);
            }

            sizer.CalculateWidths(width, options);

            var widths = columns.Select(c => c.Format.ActualWidth)
                .Concat(StackedColumnWidths(sizer))
                .ToArray();

            if (headingsRequired)
                foreach (var l in ReportHeadings(sizer.Columns, tabLength, columnDivider))
                    yield return l;

            var sizeStats = new Statistics();
            foreach (var l in ReportRowsUsedForSizing(columns, sizer, tabLength, columnDivider, widths, sizeStats))
                yield return l;

            statistics.WordWrapLineBreaks = sizeStats.WordWrapLineBreaks;

            if (enumeratorValid)
            {
                do
                {
                    var reportStats = new Statistics();
                    foreach (var l in ReportRow(dataEnumerator.Current, columns, sizer, tabLength, columnDivider, widths, reportStats))
                        yield return l;
                    statistics.WordWrapLineBreaks += reportStats.WordWrapLineBreaks;
                } while (dataEnumerator.MoveNext());
            }
        }

        private static IEnumerable<string> ReportRow(object item, List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, int tabLength, string columnSeperator, int[] widths, Statistics statistics)
        {
            var rowValues = columns
                .Select(c => GetColValue(item, c))
                .ToList();
            foreach (var l in ReportRowValues(columns, sizer, tabLength, columnSeperator, rowValues, widths, statistics))
                yield return l;
        }

        private static FormattingIntermediate GetColValue(object item, PropertyColumnFormat c)
        {
            var value = c.Property.GetValue(item);
            if (c.Format.Type == typeof (IConsoleRenderer) && value is IConsoleRenderer)
                return new FormattingIntermediate(value as IConsoleRenderer);

            return new FormattingIntermediate(ValueFormatter.Format(c.Format, value));
        }

        private static IEnumerable<string> ReportRowsUsedForSizing(List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, int tabLength, string columnSeperator, int[] widths, Statistics statistics)
        {
            foreach (var rowValues in sizer.GetSizingValues().Select(v => v.ToList()))
            {
                var stats = new Statistics();
                foreach (var l in ReportRowValues(columns, sizer, tabLength, columnSeperator, rowValues, widths, stats))
                    yield return l;
                statistics.WordWrapLineBreaks += stats.WordWrapLineBreaks;
            }
        }

        private static IEnumerable<string> ReportRowValues(List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, int tabLength, string columnSeperator, IList<FormattingIntermediate> rowValues, int[] widths, Statistics statistics)
        {
            var maxLineBreaks = 0;
            var wrappedValues = columns
                .Select((c, i) =>
                {
                    int wrappedLines;
                    var lines = RenderCol(tabLength, rowValues[i], c, out wrappedLines);
                    maxLineBreaks = Math.Max(maxLineBreaks, wrappedLines);
                    return lines;
                })
                .Concat(new Func<IEnumerable<string[]>>( () =>
                {
                    int wrappedLines;
                    var lines = StackedColumnValues(rowValues, sizer, tabLength, out wrappedLines);
                    maxLineBreaks = Math.Max(maxLineBreaks, wrappedLines);
                    return lines;
                })())
                .Select(ColourSeal.Seal)
                .ToArray();
            statistics.WordWrapLineBreaks = maxLineBreaks;

            yield return ReportColumnAligner.AlignColumns(widths, wrappedValues, ColVerticalAligment.Top, columnSeperator);
        }

        private static string[] RenderCol(int tabLength, FormattingIntermediate rowValue, PropertyColumnFormat c, out int wrappedLines)
        {
            if (c.Format.Type != typeof(IConsoleRenderer) || rowValue.RenderableValue == null)
            {
                return ColumnWrapper.WrapAndMeasureValue(rowValue.TextValue, c.Format, c.Format.ActualWidth, tabLength, 0, out wrappedLines);
            }

            var renderer = rowValue.RenderableValue;
            return renderer.Render(c.Format.ActualWidth, out wrappedLines).ToArray();
        }

        private static IEnumerable<string[]> StackedColumnValues(IEnumerable<FormattingIntermediate> rowValues, ColumnWidthNegotiator sizer, int tabLength, out int wrappedLines)
        {
            if (!sizer.StackedColumns.Any())
            {
                wrappedLines = 0;
                return new string[][] { };
            }

            var formattedLines = PropertyStackColumnFormatter.Format(sizer.StackedColumns, rowValues.Skip(sizer.Columns.Count).ToList(),
                sizer.StackedColumnWidth, tabLength)
                .ToArray();
            wrappedLines = formattedLines.Length;
            return new[] {formattedLines};
        }

        private static IEnumerable<int> StackedColumnWidths(ColumnWidthNegotiator sizer)
        {
            if (!sizer.StackedColumns.Any())
                return new int[] { };

            return new[] {sizer.StackedColumnWidth};
        }

        private static IEnumerable<string> ReportHeadings(List<PropertyColumnFormat> columns, int tabLength, string columnSeperator)
        {
            if (!columns.Any()) yield break;

            var widths = columns.Select(c => c.Format.ActualWidth).ToArray();
            var headings = columns.Select(c => WrapValue(tabLength, c, c.Format.Heading)).ToArray();
            var underlines = columns.Select(c => new [] {new string('-', c.Format.ActualWidth)}).ToArray();
            var headingLines = ReportColumnAligner.AlignColumns(widths, headings, ColVerticalAligment.Bottom, columnSeperator);
            var headingUnderLines = ReportColumnAligner.AlignColumns(widths, underlines, ColVerticalAligment.Bottom, columnSeperator);

            yield return headingLines;
            yield return headingUnderLines;
       }

        private static string[] WrapValue(int tabLength, PropertyColumnFormat pcf, object value)
        {
            var formatted = ValueFormatter.Format(pcf.Format, value);
            return ColumnWrapper.WrapValue(formatted, pcf.Format, pcf.Format.ActualWidth, tabLength);
        }
    }
}