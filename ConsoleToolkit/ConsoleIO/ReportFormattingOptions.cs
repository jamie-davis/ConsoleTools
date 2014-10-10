using System;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// Options that control the formatting of a report.
    /// </summary>
    [Flags]
    public enum ReportFormattingOptions
    {
        /// <summary>
        /// Skip the headings when emitting the report. This will cause the field names or headings for each column to be disregarded.
        /// </summary>
        OmitHeadings = 1,

        /// <summary>
        /// Fill the available width
        /// </summary>
        StretchColumns = 2,

        /// <summary>
        /// If column definitions are used, include columns without a definition.
        /// </summary>
        IncludeAllColumns = 4,

        Default = 0,
    }
}