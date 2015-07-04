namespace ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions
{
    /// <summary>
    /// Detailed options that effect the whole report, as opposed to a specific column.
    /// </summary>
    internal class ReportParameterDetails
    {
        private bool _omitHeadings;

        /// <summary>
        /// If true, this means the report should be rendered without headings.
        /// </summary>
        public bool OmitHeadings
        {
            get { return _omitHeadings; }
            set 
            { 
                _omitHeadings = value;
                RebuildOptions();
            }
        }

        private bool _stretchColumns;

        /// <summary>
        /// If true, this means the report should use all of the available space and columns should be widened, even
        /// beyond the size required by the sizing data.
        /// </summary>
        public bool StretchColumns
        {
            get { return _stretchColumns; }
            set 
            { 
                _stretchColumns = value;
                RebuildOptions();
            }
        }

        /// <summary>
        /// The options that should be specified when the report is formatted. This is generated from the settings selected.
        /// </summary>
        public ReportFormattingOptions Options { get; set; }

        /// <summary>
        /// The string that should be used to seperate columns in the report.
        /// </summary>
        public string ColumnDivider { get; set; }

        /// <summary>
        /// The number of extra spaces to indent the report.
        /// </summary>
        public int IndentSpaces { get; set; }

        /// <summary>
        /// A simple string to be shown as the report title.
        /// </summary>
        public string TitleText { get; set; }

        private void RebuildOptions()
        {
            var options = (ReportFormattingOptions)0;
            if (_omitHeadings)
                options |= ReportFormattingOptions.OmitHeadings;
            if (_stretchColumns)
                options |= ReportFormattingOptions.StretchColumns;

            Options = options;
        }
    }
}