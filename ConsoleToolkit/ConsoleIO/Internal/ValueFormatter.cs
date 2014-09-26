using System;
using System.Threading;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class ValueFormatter
    {
        public static string Format(ColumnFormat format, object value)
        {
            if (value == null) return string.Empty;

            var type = value.GetType();
            if (format.FormatTemplate != null && IsFormattable(type))
                return ApplyFormatTemplate(format.FormatTemplate, value);

            if (IsFloatingPoint(type))
            {
                var formatString = format.DecimalPlaces > 0 ? "0." + new string('0', format.DecimalPlaces) : "0";
                if (type == typeof(decimal))
                    return ((decimal)value).ToString(formatString);
                if (type == typeof(float))
                    return ((float)value).ToString(formatString);
                return ((double)value).ToString(formatString);
            }
#if TARGET_FRAMEWORK_4
            if (value is DateTime && Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "en-GB")
                return ((DateTime)value).ToString().TrimEnd(); //patch up faulty date time string format
#endif
            return value.ToString();
        }

        private static bool IsFloatingPoint(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;

                default:
                    return false;
            }
        }

        private static string ApplyFormatTemplate(string formatTemplate, object value)
        {
            var type = value.GetType();
            var toString = type.GetMethod("ToString", new[] { typeof(string) });
            if (toString == null)
                return value.ToString();

            return MethodInvoker.Invoke(toString, value, new object[] { formatTemplate }) as string;
        }

        private static bool IsFormattable(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.DateTime:
                    return true;

            }

            if (type == typeof(TimeSpan)
                || type.GetMethod("ToString", new[] { typeof(string) }) != null)
                return true;
            return false;
        }

        private static decimal ToDecimal(object value)
        {
            var typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Decimal:
                    return (decimal)value;

                case TypeCode.Single:
                    return (decimal)(float)value;

                case TypeCode.Double:
                    return (decimal)(double)value;

                default:
                    return 0m;
            }
        }
    }
}