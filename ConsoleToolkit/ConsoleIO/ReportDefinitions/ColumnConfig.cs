using System.Linq.Expressions;

namespace ConsoleToolkit.ConsoleIO.ReportDefinitions
{
    public class ColumnConfig
    {
        internal Expression ValueExpression { get; private set; }
        internal ColumnFormat ColumnFormat { get; private set; }

        internal ColumnConfig(Expression valueExpression)
        {
            ValueExpression = valueExpression;
        }

        internal void MakeFormat<TValue>()
        {
            ColumnFormat = new ColumnFormat(type: typeof(TValue));
        }

        /// <summary>
        /// Override the default column alignment for the column, and force the data to be right aligned.
        /// </summary>
        /// <returns></returns>
        public ColumnConfig RightAlign()
        {
            ColumnFormat.Alignment = ColumnAlign.Right;
            return this;
        }

        /// <summary>
        /// Override the default column alignment for the column, and force the data to be left aligned.
        /// </summary>
        /// <returns></returns>
        public ColumnConfig LeftAlign()
        {
            ColumnFormat.Alignment = ColumnAlign.Left;
            return this;
        }

        /// <summary>
        /// Sets the number of decimal places to show. This will only apply to Decimal or Double values.
        /// </summary>
        /// <returns></returns>
        public ColumnConfig DecimalPlaces(int n)
        {
            ColumnFormat.DecimalPlaces = n;
            return this;
        }

        /// <summary>
        /// Indicates that the column must have a fixed width.
        /// </summary>
        /// <returns></returns>
        public ColumnConfig Width(int n)
        {
            ColumnFormat.FixedWidth = n;
            return this;
        }

        /// <summary>
        /// Indicates that the column has a minimum width.
        /// </summary>
        /// <returns></returns>
        public ColumnConfig MinWidth(int n)
        {
            ColumnFormat.MinWidth = n;
            return this;
        }
        /// <summary>
        /// Indicates that the column has a maximum width.
        /// </summary>
        /// <returns></returns>
        public ColumnConfig MaxWidth(int n)
        {
            ColumnFormat.MaxWidth = n;
            return this;
        }

        /// <summary>
        /// Set the heading text for the column.
        /// </summary>
        public ColumnConfig Heading(string heading)
        {
            ColumnFormat.Heading = heading;
            return this;
        }

        /// <summary>
        /// This is called after the column has been configured and will default any crucial settings that have not been configured.
        /// </summary>
        public void FinalizeColumnSettings()
        {
            if (ColumnFormat.Heading == null)
                ColumnFormat.Heading = ColumnNameExtractor.FromExpression(ValueExpression);
        }
    }
}