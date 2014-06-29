using System;
using System.Collections;
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

            Task.Factory.StartNew(() =>
            {
                try
                {
                    int localWordWrapLineBreaks;
                    DoFormat(output, data, columns, width, numRowsToUseForSizing, 4, out localWordWrapLineBreaks);
                }
                catch (Exception e)
                {
                    output.Add("Exception while processing:");
                    output.Add(e.ToString());
                }
                finally
                {
                    output.CompleteAdding();
                }
            });

            return output.GetConsumingEnumerable();
        }

        /// <summary>
        /// Format a collection of cached rows as a tabular report.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="data">The cached row items.</param>
        /// <param name="columns">Column formatting information. If this is not provided, default column formats will be used.</param>
        /// <param name="width">The width that the report is allowed to occupy.</param>
        /// <returns>The formatted report lines.</returns>
        public static IEnumerable<string> Format<T>(CachedRows<T> data, IEnumerable<ColumnFormat> columns, int width)
        {
            var output = new BlockingCollection<string>();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    int wordWrapLineBreaks;
                    DoFormatFromCachedRows(output, data, columns, width, 0, 4, out wordWrapLineBreaks);
                }
                catch (Exception e)
                {
                    output.Add("Exception while processing:");
                    output.Add(e.ToString());
                }
                finally
                {
                    output.CompleteAdding();
                }
            });

            return output.GetConsumingEnumerable();
        }

        /// <summary>
        /// Format a collection of cached rows as a tabular report.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="data">The cached row items.</param>
        /// <param name="columns">Column formatting information. If this is not provided, default column formats will be used.</param>
        /// <param name="width">The width that the report is allowed to occupy.</param>
        /// <param name="wrappingLineBreaks">The number of linebreaks added due to wrapping.</param>
        /// <returns>The formatted report lines.</returns>
        public static IEnumerable<string> Format<T>(CachedRows<T> data, IEnumerable<ColumnFormat> columns, int width, out int wrappingLineBreaks)
        {
            var output = new BlockingCollection<string>();
            var addedLineBreaks = 0;

            try
            {
                int wordWrapLineBreaks;
                DoFormatFromCachedRows(output, data, columns, width, 0, 4, out wordWrapLineBreaks);
                addedLineBreaks = wordWrapLineBreaks;
                Console.WriteLine("Added line breaks = {0}", addedLineBreaks);
            }
            catch (Exception e)
            {
                output.Add("Exception while processing:");
                output.Add(e.ToString());
            }
            finally
            {
                Console.WriteLine("Completed adding");
                output.CompleteAdding();
            }

            var lines = output.GetConsumingEnumerable().ToList();
            wrappingLineBreaks = addedLineBreaks;
            return lines;
        }

        private static void DoFormatFromCachedRows<T>(BlockingCollection<string> output, CachedRows<T> data, IEnumerable<ColumnFormat> specifiedColumns, int width, int numRowsToUseForSizing, int tabLength, out int wordWrapLineBreaks)
        {
            var columns = FormatAnalyser.Analyse(typeof(T), specifiedColumns);
            FormatFromDataFeed<CachedRow<T>>(output, data.GetRows(), width, numRowsToUseForSizing, tabLength, columns, (sz, item) => sz.AddRow(item), out wordWrapLineBreaks);
        }

        private static void DoFormat<T>(BlockingCollection<string> output, IEnumerable<T> data, IEnumerable<ColumnFormat> specifiedColumns, int width, int numRowsToUseForSizing, int tabLength, out int wordWrapLineBreaks)
        {
            var columns = FormatAnalyser.Analyse(typeof(T), specifiedColumns);
            FormatFromDataFeed<T>(output, data, width, numRowsToUseForSizing, tabLength, columns, (sz, item) => sz.AddRow(item), out wordWrapLineBreaks);
        }

        private static void FormatFromDataFeed<T>(BlockingCollection<string> output,
            IEnumerable data,
            int width, int numRowsToUseForSizing, int tabLength,
            List<PropertyColumnFormat> columns, Action<ColumnWidthNegotiator, T> addRowAction,
            out int wordWrapLineBreaks)
        {
            var sizer = new ColumnWidthNegotiator(columns, 1);

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

            sizer.CalculateWidths(width);

            const string columnSeperator = " ";
            var widths = columns.Select(c => c.Format.ActualWidth)
                .Concat(StackedColumnWidths(sizer))
                .ToArray();

            ReportHeadings(sizer.Columns, output, tabLength, columnSeperator);
            int sizeRowsWordWrapLineBreaks;
            ReportRowsUsedForSizing(columns, sizer, output, tabLength, columnSeperator, widths,
                out sizeRowsWordWrapLineBreaks);

            wordWrapLineBreaks = sizeRowsWordWrapLineBreaks;

            if (enumeratorValid)
            {
                do
                {
                    int reportWordWrapLineBreaks;
                    ReportRow(dataEnumerator.Current, columns, sizer, output, tabLength, columnSeperator, widths,
                        out reportWordWrapLineBreaks);
                    wordWrapLineBreaks += reportWordWrapLineBreaks;
                } while (dataEnumerator.MoveNext());
            }
        }

        private static void ReportRow(object item, List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, BlockingCollection<string> output, int tabLength, string columnSeperator, int[] widths, out int reportWordWrapLineBreaks)
        {
            var rowValues = columns
                .Select(c => GetColValue(item, c))
                .ToList();
            ReportRowValues(columns, sizer, output, tabLength, columnSeperator, rowValues, widths, out reportWordWrapLineBreaks);
        }

        private static FormattingIntermediate GetColValue(object item, PropertyColumnFormat c)
        {
            var value = c.Property.GetValue(item);
            if (c.Format.Type == typeof (IConsoleRenderer) && value is IConsoleRenderer)
                return new FormattingIntermediate(value as IConsoleRenderer);

            return new FormattingIntermediate(ValueFormatter.Format(c.Format, value));
        }

        private static void ReportRowsUsedForSizing(List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, BlockingCollection<string> output, int tabLength, string columnSeperator, int[] widths, out int sizeRowsWordWrapLineBreaks)
        {
            sizeRowsWordWrapLineBreaks = 0;
            foreach (var rowValues in sizer.GetSizingValues().Select(v => v.ToList()))
            {
                int wordWrapLineBreaks;
                ReportRowValues(columns, sizer, output, tabLength, columnSeperator, rowValues, widths, out wordWrapLineBreaks);
                sizeRowsWordWrapLineBreaks += wordWrapLineBreaks;
            }
        }

        private static void ReportRowValues(List<PropertyColumnFormat> columns, ColumnWidthNegotiator sizer, BlockingCollection<string> output, int tabLength, string columnSeperator, IReadOnlyList<FormattingIntermediate> rowValues, int[] widths, out int reportWordWrapLineBreaks)
        {
            reportWordWrapLineBreaks = 0;
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
            reportWordWrapLineBreaks = maxLineBreaks;
            output.Add(ReportColumnAligner.AlignColumns(widths, wrappedValues, ColVerticalAligment.Top, columnSeperator));
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

        private static IEnumerable<string[]> StackedColumnValues(IEnumerable<object> rowValues, ColumnWidthNegotiator sizer, int tabLength, out int wrappedLines)
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