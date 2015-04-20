using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.Utilities
{
    /// <summary>
    /// A utility for analysing and extracting data from a <see cref="Report{T}"/>.
    /// <para/>
    /// Be aware that the report will be enumerated by this class and so it cannot also be formatted.
    /// <remarks><para>Renderable column types are not supported. (For example, <see cref="RecordingConsoleAdapter"/>.)</para></remarks>
    /// <remarks>Please note that this class is experimental and may change radically or be removed in a future version.</remarks>
    /// </summary>
    public class ReportAnalyser
    {
        /// <summary>
        /// Construct an analyser for a report. The report will be enumerated by this class.
        /// </summary>
        /// <param name="report">The report to analyse.</param>
        public static ReportAnalysis<T> Analyse<T>(Report<T> report)
        {
            return new ReportAnalysis<T>(report);
        }
    }

    /// <summary>
    /// The results of an analysis of a report.
    /// <para/>
    /// Be aware that the report will be enumerated by this class and so it cannot also be formatted.
    /// </summary>
    public class ReportAnalysis<T>
    {
        private readonly Report<T> _report;
        private List<string> _columns;
        private IEnumerator _enumerator;
        private List<PropertyColumnFormat> _colFormats;

        internal ReportAnalysis(Report<T> report)
        {
            _report = report;
            _enumerator = _report.Query.GetEnumerator();
            _colFormats = FormatAnalyser.Analyse(_report.RowType, _report.Columns, false);

            _columns = report.Columns.Select(c => c.Heading).ToList();
        }

        public IEnumerable<string> Columns { get { return _columns; } }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public string GetColumnValue(string columnName)
        {
            var format = _colFormats.FirstOrDefault(f => f.Format.Heading == columnName);
            if (format == null)
                return string.Empty;

            var value = format.Property.GetValue(_enumerator.Current);
            if (value is IConsoleRenderer)
                return string.Empty;

            return ValueFormatter.Format(format.Format, value);
        }
    }
}
