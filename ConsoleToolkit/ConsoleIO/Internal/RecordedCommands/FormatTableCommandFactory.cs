using System.Collections.Generic;

namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal static class FormatTableCommandFactory
    {
        internal static FormatTableCommand<T> Make<T>(IEnumerable<T> data, ReportFormattingOptions options = ReportFormattingOptions.None)
        {
            return new FormatTableCommand<T>(data, options);
        }
    }
}