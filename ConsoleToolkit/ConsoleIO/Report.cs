using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// A wrapper around an enumerable set of objects to be displayed as a report.
    /// </summary>
    /// <typeparam name="T">The item type of the input enumerable.</typeparam>
    public class Report<T>
    {
        private List<ColumnFormat> _columns;
        private IEnumerable _query;
        private Type _output;
        private List<BaseChildItem<T>> _children;

        public Report(IEnumerable<T> items, ReportParameters<T> reportParameters)
        {
            _columns = reportParameters.ColumnSource.Columns.ToList();
            Options = reportParameters.Details.Options;
            ColumnDivider = reportParameters.Details.ColumnDivider;
            _children = reportParameters.Children;

            Func<object, T> rowGetter;
            _query = ReportQueryBuilder.Build(items, reportParameters.ColumnConfigs.Select(c => c.ValueExpression), out _output, out rowGetter);
            foreach (var baseChildItem in _children)
            {
                baseChildItem.SetOriginalRowExtractor(rowGetter);
            }
        }

        /// <summary>
        /// The derived report query.
        /// </summary>
        internal IEnumerable Query { get { return _query; } }

        /// <summary>
        /// The type that is returned by the Query object.
        /// </summary>
        internal Type RowType { get { return _output; } }

        /// <summary>
        /// The custom formatters for each column in the report.
        /// </summary>
        internal IEnumerable<ColumnFormat> Columns { get { return _columns; } }

        /// <summary>
        /// The defintions of this report's children.
        /// </summary>
        internal IEnumerable<BaseChildItem<T>> Children { get { return _children; } }

        /// <summary>
        /// The report options to apply for this report.
        /// </summary>
        internal ReportFormattingOptions Options { get; private set; }

        /// <summary>
        /// The divider to place between each column on the report.
        /// </summary>
        internal string ColumnDivider { get; private set; }
    }
}