using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class HeadingManager
    {
        private readonly bool _headingsRequired;
        private readonly bool _allowChildReportHeadingRepeats;
        private bool _showHeadings = true;
        private List<PropertyColumnFormat> _columns;
        private int _tabLength;
        private string _columnDivider;

        public bool ShowHeadings { get { return _headingsRequired && _showHeadings; } }

        public HeadingManager(bool headingsRequired, bool allowChildReportHeadingRepeats)
        {
            _headingsRequired = headingsRequired;
            _allowChildReportHeadingRepeats = allowChildReportHeadingRepeats;
        }

        public void HeadingsParameters(List<PropertyColumnFormat> columns, int tabLength, string columnDivider)
        {
            _columns = columns;
            _tabLength = tabLength;
            _columnDivider = columnDivider;
        }

        public IEnumerable<string> ReportHeadings()
        {
            return ReportHeadings(_columns, _tabLength, _columnDivider);
        }


        private IEnumerable<string> ReportHeadings(List<PropertyColumnFormat> columns, int tabLength, string columnSeperator)
        {
            _showHeadings = false;

            if (!columns.Any()) yield break;

            var widths = columns.Select(c => c.Format.ActualWidth).ToArray();
            var headings = columns.Select(c => WrapValue(tabLength, c, c.Format.Heading)).ToArray();
            var underlines = columns.Select(c => new[] { new string('-', c.Format.ActualWidth) }).ToArray();
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

        public void ChildGenerated()
        {
            if (_allowChildReportHeadingRepeats)
                _showHeadings = true;
        }
    }
}
