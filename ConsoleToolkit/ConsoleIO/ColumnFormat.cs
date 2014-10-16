using System;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class is used to format values that are to be displayed in a column.
    /// </summary>
    public class ColumnFormat
    {
        public string Heading { get; internal set; }
        public Type Type { get; private set; }
        public ColumnAlign Alignment { get; internal set; }
        public int DecimalPlaces { get; internal set; }
        public int ActualWidth { get; private set; }
        public string FormatTemplate { get; private set; }
        public string Width { get; private set; }
        public int FixedWidth { get; set; }
        public int MinWidth { get; set; }
        public int MaxWidth { get; set; }

        /// <summary>
        /// Constructor. All parameters other than heading are optional, and designed to be set by name. For example:
        /// 
        ///     var format = new ColumnFormat("My Col", alignment: ColumnAlign.Right);
        /// </summary>
        /// <param name="heading">The heading text for the column</param>
        /// <param name="type">The type of the data expected for the column. When specified, this allows some parameters to be given sensible defaults.</param>
        /// <param name="alignment">Whether the column is left or right aligned. If <see cref="ColumnAlign.Auto"/> is set, it will be right aligned for numeric types, or else left aligned. 
        /// If no value is provided for <see cref="type"/>, left will be used.</param>
        /// <param name="decimalPlaces">The number of decimal places to show. This will only apply to Decimal or Double values.</param>
        /// <param name="format">A .NET format string that will be applied to the data. If this is specified, the decimal places setting will be ignored.</param>
        /// <param name="width">A string describing the width required for this column. If a string containing a number is provided, this will be the columns fixed width, if an asterisk is provided, this column will be as wide as possible. If no value is provided, the column's actual width will be determined from its contents.</param>
        public ColumnFormat(string heading = null, Type type = null, ColumnAlign alignment = ColumnAlign.Auto, 
            int decimalPlaces = 2, string format = null, string width = null)
        {
            Heading = heading;
            Type = type;
            Alignment = alignment;
            DecimalPlaces = decimalPlaces;
            FormatTemplate = format;
            Width = width;

            CalculateAutoParameters();
        }

        private void CalculateAutoParameters()
        {
            if (Type != null)
            {
                if (Alignment == ColumnAlign.Auto)
                {
                    if (IsNumeric(Type))
                        Alignment = ColumnAlign.Right;
                    else
                        Alignment = ColumnAlign.Left;
                }
            }
        }

        public bool DetermineWidthFromData()
        {
            return FixedWidth == 0;
        }

        private bool IsNumeric(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;

                default:
                    return false;
            }
        }

        internal void SetActualWidth(int actualWidth)
        {
            ActualWidth = actualWidth;
        }

        public override string ToString()
        {
            return string.Format("ColumnFormat(\"{0}\", {1}, {2}, {3}DP, Actual {4}, {5}, Requested {6})", Heading, Type, Alignment, DecimalPlaces, ActualWidth, FormatTemplate, Width);
        }
    }
}
