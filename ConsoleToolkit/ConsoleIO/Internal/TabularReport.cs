using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// A formatter for tabular data.
    /// </summary>
    internal static class TabularReport
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
        /// <typeparam name="TChild">The original row type for child reports.</typeparam>
        /// <param name="data">The enumerable items.</param>
        /// <param name="columns">Column formatting information. If this is not provided, default column formats will be used.</param>
        /// <param name="width">The width that the report is allowed to occupy.</param>
        /// <param name="numRowsToUseForSizing">The number of rows that should be used for automatically sizing columns.</param>
        /// <param name="options">Options that control the formatting of the report.</param>
        /// <param name="columnDivider">A string that will be used to divide columns.</param>
        /// <param name="childReports">The nested reports that should be output for each table row.</param>
        /// <returns>The formatted report lines.</returns>
        public static IEnumerable<string> Format<T, TChild>(IEnumerable<T> data, IEnumerable<ColumnFormat> columns,
            int width, int numRowsToUseForSizing = 0, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnDivider = null, IEnumerable<BaseChildItem<TChild>> childReports = null)
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            using (new TempCulture(culture))
            {
                    var statistics = new Statistics();
                    foreach (var l in DoFormat(data, columns, width, numRowsToUseForSizing, 4, options, columnDivider, statistics, childReports))
                        yield return l;
            }
        }

        /// <summary>
        /// Format a collection of rows as a tabular report.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <typeparam name="TChild">The original row type for child reports.</typeparam>
        /// <param name="data">The cached row items.</param>
        /// <param name="columns">Column formatting information. If this is not provided, default column formats will be used.</param>
        /// <param name="width">The width that the report is allowed to occupy.</param>
        /// <param name="statistics">An object that captures statistics about the formatting process.</param>
        /// <param name="options">Options that control the formatting of the report.</param>
        /// <param name="columnDivider">A string that will be used to divide columns.</param>
        /// <param name="childReports">The nested reports that should be output for each table row.</param>
        /// <returns>The formatted report lines.</returns>
        public static IEnumerable<string> Format<T, TChild>(CachedRows<T> data, IEnumerable<ColumnFormat> columns,
            int width, Statistics statistics, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnDivider = null, IEnumerable<BaseChildItem<TChild>> childReports = null)
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            using (new TempCulture(culture))
            {
                    foreach (var l in DoFormatFromCachedRows(data, columns, width, 0, 4, options, columnDivider, statistics, childReports))
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
        /// <param name="childReports">The nested reports that should be output for each table row.</param>
        /// <returns>The formatted report lines.</returns>
        public static IEnumerable<string> Format<T>(CachedRows<T> data, IEnumerable<ColumnFormat> columns, 
            int width, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnDivider = null, IEnumerable<BaseChildItem<T>> childReports = null)
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            using (new TempCulture(culture))
            {
                var statistics = new Statistics();
                foreach (var l in DoFormatFromCachedRows(data, columns, width, 0, 4, options, columnDivider, statistics, childReports))
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
        /// <param name="childReports">The nested reports that should be output for each table row.</param>
        /// <returns>The formatted report lines.</returns>
        internal static IEnumerable<string> Format<T>(CachedRows<T> data, IEnumerable<ColumnFormat> columns, int width, Statistics wrappingLineBreaks, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnDivider = null, IEnumerable<BaseChildItem<T>> childReports = null)
        {
            var addedLineBreaks = 0;

            var statistics = new Statistics();
            foreach (var l in DoFormatFromCachedRows(data, columns, width, 0, 4, options, columnDivider, statistics, childReports))
                yield return l;
            addedLineBreaks = statistics.WordWrapLineBreaks;

            wrappingLineBreaks.WordWrapLineBreaks = addedLineBreaks;
        }

        private static IEnumerable<string> DoFormatFromCachedRows<T, TChild>(CachedRows<T> data, IEnumerable<ColumnFormat> specifiedColumns, int width, int numRowsToUseForSizing, int tabLength, ReportFormattingOptions options, string columnDivider, Statistics statistics, IEnumerable<BaseChildItem<TChild>> childReports)
        {
            var columns = FormatAnalyser.Analyse(typeof(T), specifiedColumns, IncludeAllColumns(specifiedColumns, options));
            Action<ColumnWidthNegotiator, CachedRow<T>> addRowAction = (sz, item) => sz.AddRow(item);

            return FormatFromDataFeed(data.GetRows(), width, numRowsToUseForSizing, 
                tabLength, columns, addRowAction, options, columnDivider, statistics,
                childReports);
        }

        private static IEnumerable<string> DoFormat<T, TChild>(IEnumerable<T> data, IEnumerable<ColumnFormat> specifiedColumns, int width, int numRowsToUseForSizing, int tabLength, ReportFormattingOptions options, string columnDivider, Statistics statistics, IEnumerable<BaseChildItem<TChild>> childReports)
        {
            var columns = FormatAnalyser.Analyse(typeof(T), specifiedColumns, IncludeAllColumns(specifiedColumns, options));
            Action<ColumnWidthNegotiator, T> addRowAction = (sz, item) => sz.AddRow(item);

            return FormatFromDataFeed(data, width, numRowsToUseForSizing, tabLength, columns, addRowAction, options, columnDivider, statistics, childReports);
        }

        private static bool IncludeAllColumns(IEnumerable<ColumnFormat> specifiedColumns, ReportFormattingOptions options)
        {
            return specifiedColumns == null || options.HasFlag(ReportFormattingOptions.IncludeAllColumns);
        }

        private static IEnumerable<string> FormatFromDataFeed<T, TChild>(IEnumerable data, int width, int numRowsToUseForSizing, 
            int tabLength, List<PropertyColumnFormat> columns, Action<ColumnWidthNegotiator, T> addRowAction, 
            ReportFormattingOptions options, string columnDivider, Statistics statistics,
            IEnumerable<BaseChildItem<TChild>> childReports)
        {
            if (columnDivider == null) columnDivider = DefaultColumnDivider;

            var sizer = new ColumnWidthNegotiator(columns, columnDivider.Length);

            var headingsControl = new HeadingManager((options & ReportFormattingOptions.OmitHeadings) == 0);
            if (headingsControl.ShowHeadings)
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

            if (headingsControl.ShowHeadings)
            {
                headingsControl.HeadingsParameters(sizer.Columns, tabLength, columnDivider);
                foreach (var l in headingsControl.ReportHeadings())
                    yield return l;
            }

            var sizeStats = new Statistics();
            foreach (var l in ReportRowsUsedForSizing<T, TChild>(columns, sizer, tabLength, columnDivider, widths, sizeStats, childReports, headingsControl))
                yield return l;

            statistics.WordWrapLineBreaks = sizeStats.WordWrapLineBreaks;

            if (enumeratorValid)
            {
                do
                {
                    var reportStats = new Statistics();
                    foreach (var l in ReportRow<T, TChild>(dataEnumerator.Current, columns, sizer, tabLength, columnDivider, widths, reportStats, childReports, headingsControl))
                        yield return l;
                    statistics.WordWrapLineBreaks += reportStats.WordWrapLineBreaks;
                } while (dataEnumerator.MoveNext());
            }
        }

        private static IEnumerable<string> ReportRow<T, TChild>(object item, List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, int tabLength, string columnSeperator, int[] widths, Statistics statistics, IEnumerable<BaseChildItem<TChild>> children, HeadingManager headingsControl)
        {
            var rowValues = columns
                .Select(c => GetColValue(item, c))
                .ToList();
            foreach (var l in ReportRowValues<T, TChild>(columns, sizer, tabLength, columnSeperator, item, rowValues, widths, statistics, children, headingsControl))
                yield return l;
        }

        private static FormattingIntermediate GetColValue(object item, PropertyColumnFormat c)
        {
            object value;
            if (c.Property == null)
                value = item;
            else
                value = c.Property.GetValue(item);

            if (c.Format.Type == typeof (IConsoleRenderer) && value is IConsoleRenderer)
                return new FormattingIntermediate(value as IConsoleRenderer);

            return new FormattingIntermediate(ValueFormatter.Format(c.Format, value));
        }

        private static IEnumerable<string> ReportRowsUsedForSizing<T, TChild>(List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, int tabLength, string columnSeperator, int[] widths, Statistics statistics, IEnumerable<BaseChildItem<TChild>> children, HeadingManager headingsControl)
        {
            foreach (var rowValues in sizer.GetSizingValues())
            {
                var stats = new Statistics();
                var columnValues = rowValues.GetValues().ToList();
                foreach (var l in ReportRowValues<T, TChild>(columns, sizer, tabLength, columnSeperator, rowValues.RowItem, columnValues, widths, stats, children, headingsControl))
                    yield return l;
                statistics.WordWrapLineBreaks += stats.WordWrapLineBreaks;
            }
        }

        private static IEnumerable<string> ReportRowValues<T, TChild>(List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, int tabLength, string columnSeperator, object rowItem, IList<FormattingIntermediate> rowValues, int[] widths, Statistics statistics, IEnumerable<BaseChildItem<TChild>> children, HeadingManager headingsControl)
        {
            if (headingsControl.ShowHeadings && children != null && children.Any())
            {
                foreach (var heading in headingsControl.ReportHeadings())
                {
                    yield return heading;
                }
            }

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

            if (children != null)
            {
                var childRendered = false;
                foreach (var baseChildItem in children)
                {
                    var childOutput = PrefixLines.Do(baseChildItem.Render(rowItem, sizer.AvailableWidth - 4), "    ");
                    if (string.IsNullOrEmpty(childOutput))
                        continue;

                    childRendered = true;
                    yield return Environment.NewLine;
                    yield return childOutput;
                }

                if (childRendered)
                {
                    yield return Environment.NewLine;
                    headingsControl.ChildGenerated();
                }
            }
        }

        private static string[] RenderCol(int tabLength, FormattingIntermediate rowValue, PropertyColumnFormat c, out int wrappedLines)
        {
            if (rowValue.RenderableValue == null)
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
    }
}