using System.Collections.Generic;

namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal static class FormatTableCommandFactory
    {
        internal static FormatTableCommand<T> Make<T>(IEnumerable<T> data, string columnSeperator = null, ReportFormattingOptions options = ReportFormattingOptions.Default)
        {
            return new FormatTableCommand<T>(data, options, columnSeperator ?? " ");
        }
    }
}