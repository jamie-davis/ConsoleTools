using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal static class FormatTableCommandFactory
    {
        public static FormatTableCommand<T, T> Make<T>(IEnumerable<T> data, string columnSeperator = null, ReportFormattingOptions options = ReportFormattingOptions.Default, IEnumerable<BaseChildItem<T>> childReports = null, IEnumerable<ColumnFormat> columns = null)
        {
            return new FormatTableCommand<T, T>(data, options, columnSeperator ?? " ", columns: columns, childReports: childReports);
        }

        internal static IRecordedCommand Make<T>(Report<T> report)
        {
            var itemType = report.RowType;
            var parameters = new Object[]
                             {
                                report
                             };
            var genericMethod = typeof(FormatTableCommandFactory).GetMethod("CallMake", BindingFlags.NonPublic | BindingFlags.Static);

            var method = genericMethod.MakeGenericMethod(typeof(T), itemType);

            return MethodInvoker.Invoke(method, null, parameters) as IRecordedCommand;
        }

        // ReSharper disable once UnusedMember.Global
        internal static FormatTableCommand<TReportItem, TOriginal> CallMake<TOriginal, TReportItem>(Report<TOriginal> report)
        {
            return new FormatTableCommand<TReportItem, TOriginal>(report.Query.Cast<TReportItem>(), report.Options, report.ColumnDivider, report.Children, report.Columns);
        }
    }
}