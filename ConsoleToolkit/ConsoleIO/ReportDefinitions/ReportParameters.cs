using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions;

namespace ConsoleToolkit.ConsoleIO.ReportDefinitions
{
    public class ReportParameters<T>
    {
        private ColumnSource _columnSource = new ColumnSource();
        internal ReportParameterDetails Details { get; set; }
        internal List<ColumnConfig> ColumnConfigs { get; set; }

        public ReportParameters()
        {
            Details = new ReportParameterDetails();
            ColumnConfigs = new List<ColumnConfig>();
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
}
