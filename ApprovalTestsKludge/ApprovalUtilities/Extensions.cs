using System;
using System.Collections.Generic;
using System.Text;

namespace ApprovalUtilities.Utilities
{
    public static class Extensions
    {
        public static string JoinWith(this IEnumerable<string> input, string concatentator)
        {
            return string.Join(concatentator, input);
        }
        
        public static string WritePropertiesToString<T>(this T value)
        {
            return WriteObjectToString(value, WriteProperties);
        }

        private static void WriteProperties<T>(T value, StringBuilder sb, Type t)
        {
            foreach (var p in t.GetProperties())
            {
                if (p.CanRead)
                {
                    var propertyValue = p.GetValue(value, new object[0]) ?? "NULL";
                    sb.AppendFormat("\t{0}: {1}", p.Name, propertyValue).AppendLine();
                }
            }
        }

        public static string WriteFieldsToString<T>(this T value)
        {
            return WriteObjectToString(value, WriteFields);
        }

        private static void WriteFields<T>(T value, StringBuilder sb, Type t)
        {
            foreach (var f in t.GetFields())
            {
                if (f.IsPublic)
                {
                    var propertyValue = f.GetValue(value) ?? "NULL";
                    sb.AppendFormat("\t{0}: {1}", f.Name, propertyValue).AppendLine();
                }
            }
        }

        private static string WriteObjectToString<T>(T value, Action<T, StringBuilder, Type> writer)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var t = typeof (T);
            var sb = new StringBuilder();
            sb.AppendLine(t.Name);
            sb.AppendLine("{");
            writer(value, sb, t);

            sb.AppendLine("}");

            return sb.ToString();
        }

    }
}