using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions;

namespace ConsoleToolkit.ConsoleIO.ReportDefinitions
{
    public class ReportParameters<T>
    {
        private ColumnSource _columnSource = new ColumnSource();
        internal ReportParameterDetails Details { get; set; }
        internal List<ColumnConfig> ColumnConfigs { get; set; }
        internal List<BaseChildItem<T>> Children { get; set; } 

        public ReportParameters()
        {
            Details = new ReportParameterDetails();
            ColumnConfigs = new List<ColumnConfig>();
            Children = new List<BaseChildItem<T>>();
        }
        
        public ReportParameters<T> AddColumn<TValue>(Expression<Func<T, TValue>> valueExpression, Action<ColumnConfig> defineCol)
        {
            var columnConfig = new ColumnConfig(valueExpression);
            columnConfig.MakeFormat<TValue>();
            defineCol(columnConfig);
            columnConfig.FinalizeColumnSettings();
            _columnSource.AddColumn(columnConfig);
            ColumnConfigs.Add(columnConfig);
            return this;
        }
        
        public ReportParameters<T> AddChild<TValueItem>(Func<T, IEnumerable<TValueItem>> childData, Action<ReportParameters<TValueItem>> childReport)
        {
            var reportParameters = new ReportParameters<TValueItem>();
            childReport(reportParameters);
            var child = new Child<T, TValueItem>(childData, reportParameters);
            Children.Add(child);
            return this;
        }

        public ReportParameters<T> OmitHeadings()
        {
            Details.OmitHeadings = true;
            return this;
        }

        internal ColumnSource ColumnSource { get { return _columnSource; } }

        public ReportParameters<T> StretchColumns()
        {
            Details.StretchColumns = true;
            return this;
        }

        /// <summary>
        /// Specify the number of extra spaces to indent the report.
        /// </summary>
        /// <param name="spaces">The extra indent spaces count.</param>
        /// <returns>The parameter object.</returns>
        public ReportParameters<T> Indent(int spaces)
        {
            Details.IndentSpaces = spaces;
            return this;
        }
    }

    internal abstract class BaseChildItem<T>
    {
        public abstract string Render(object item, int width);

        public abstract void SetOriginalRowExtractor(Func<object, T> rowGetter);
    }

    internal sealed class Child<T, TValueItem> : BaseChildItem<T>
    {
        private readonly Func<T, IEnumerable<TValueItem>> _childDataAccessor;
        private readonly ReportParameters<TValueItem> _reportParameters;
        private Func<object, T> _originalRowExtractor;

        public Child(Func<T, IEnumerable<TValueItem>> childDataAccessor, ReportParameters<TValueItem> reportParameters)
        {
            _childDataAccessor = childDataAccessor;
            _reportParameters = reportParameters;
        }

        public override void SetOriginalRowExtractor(Func<object, T> originalRowExtractor)
        {
            _originalRowExtractor = originalRowExtractor;
        }

        public override string Render(object item, int width)
        {
            var childData = _childDataAccessor(_originalRowExtractor(item));
            var report = new Report<TValueItem>(childData, _reportParameters);
            return string.Join(string.Empty, ReportExecutor.GetLines(report, width));
        }
    }
}
