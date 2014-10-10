using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
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
    }

    internal abstract class BaseChildItem<T>
    {
        public abstract string Render(object item, int width);

        public abstract void SetOriginalRowExtractor(Func<object, T> rowGetter);
    }

    internal sealed class CachedRowChild<T> : BaseChildItem<CachedRow<T>>
    {
        private readonly BaseChildItem<T> _child;

        public CachedRowChild(BaseChildItem<T> child)
        {
            _child = child;
        }

        public override string Render(object item, int width)
        {
            var cachedRowItem = (CachedRow<T>) item;
            return _child.Render(cachedRowItem.RowItem, width);
        }

        public override void SetOriginalRowExtractor(Func<object, CachedRow<T>> rowGetter)
        {
            Func<object, T> childRowGetter = o => rowGetter(o).RowItem;
            _child.SetOriginalRowExtractor(childRowGetter);
        }
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
            var tabular = TabularReport.Format(_childDataAccessor(_originalRowExtractor(item)), 
                _reportParameters.ColumnSource.Columns, 
                width, 
                0, //num rows to use for sizing
                _reportParameters.Details.Options, 
                _reportParameters.Details.ColumnDivider, 
                _reportParameters.Children);
            return string.Join(string.Empty, tabular);
        }
    }

    internal class ChildConfig
    {
        
    }
}
