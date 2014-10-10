using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;

namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    /// <summary>
    /// A recorded format table command.
    /// </summary>
    /// <remarks>A recorded format table command has slightly different rules to normal text. 
    /// Since it cannot be wrapped correctly, it would be unhelpful to break it into words, therefore
    /// questions like "What is the longest word?" or "How long is the first word?" are really
    /// academic, and are therefore given the best answer available, which is, what is the narrowest
    /// the table can be? This is not really a good answer, however, as of this writing, it can only
    /// be asked as part of a <see cref="PropertyStackFormatter"/> related operation, in which case the
    /// space available for the table will be minimal and the table will be rendered on a new line.
    /// <p/>
    /// The longest word will be used to set the stack column width, and therefore the table should
    /// be able to render itself correctly. However, if it cannot and it must resort to stacking, then
    /// the result will not be very readable. That is the consequence of having a lot of data but very 
    /// little space.</remarks>
    /// <typeparam name="T">The table row data type.</typeparam>
    internal class FormatTableCommand<T> : IRecordedCommand
    {
        private readonly ReportFormattingOptions options;
        private readonly IEnumerable<BaseChildItem<T>> _childReports;
        private readonly string _columnSeperator;
        private readonly CachedRows<T> _data;
        private int _minReportWidth;

        public FormatTableCommand(IEnumerable<T> data, ReportFormattingOptions options, string columnSeperator, IEnumerable<BaseChildItem<T>> childReports = null)
        {
            this.options = options;
            _childReports = childReports == null ? null : childReports.ToList();
            _columnSeperator = columnSeperator ?? TabularReport.DefaultColumnDivider;
            _data = CachedRowsFactory.Make(data);
            _minReportWidth = MinReportWidthCalculator.Calculate(_data, _columnSeperator.Length);
        }

        public void Replay(ReplayBuffer buffer)
        {
            var  wrappedLineBreaks = new TabularReport.Statistics();
            var report = TabularReport.Format(_data, null, buffer.Width, wrappedLineBreaks, options, _columnSeperator, _childReports);
            foreach (var line in report)
            {
                buffer.Write(line);
            }
            buffer.RecordWrapLineBreaks(wrappedLineBreaks.WordWrapLineBreaks);
        }

        public int GetFirstWordLength(int tabLength)
        {
            return _minReportWidth;
        }

        public int GetLongestWordLength(int tabLength)
        {
            return _minReportWidth;
        }
    }
}