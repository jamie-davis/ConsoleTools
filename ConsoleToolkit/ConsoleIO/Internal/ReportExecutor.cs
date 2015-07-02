using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Execute a <see cref="Report{T}"/>.
    /// </summary>
    internal static class ReportExecutor
    {
        /// <summary>
        /// Return the lines output by a report.
        /// </summary>
        /// <typeparam name="T">The type of the items that are input to the report.</typeparam>
        /// <param name="report">The report definition.</param>
        /// <param name="availableWidth">The available width for formatting.</param>
        /// <returns>A set of report lines.</returns>
        internal static IEnumerable<string> GetLines<T>(Report<T> report, int availableWidth)
        {
            int width;
            var indent = string.Empty;
            if (availableWidth > report.IndentSpaceCount)
                width = availableWidth - report.IndentSpaceCount;
            else
                width = availableWidth;
            var actualIndent = availableWidth - width;

            var formatMethod = MakeFormatMethodInfo(report);
            var parameters = new object[]
                             {
                                 report.Query,
                                 report.Columns,
                                 width,
                                 0, //rows to use for sizing
                                 report.Options,
                                 report.ColumnDivider,
                                 report.Children
                             };

            var tabular = MethodInvoker.Invoke(formatMethod, null, parameters) as IEnumerable<string>;
            if (actualIndent > 0)
                indent = new string(' ', actualIndent);
            foreach (var line in tabular)
            {
                if (actualIndent > 0)
                    yield return indent + line;
                else
                    yield return line;
            }
        }

        private static MethodInfo MakeFormatMethodInfo<T>(Report<T> report)
        {
            var genericMethod = typeof(TabularReport)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == "Format"
                                     && m.GetParameters()[0].ParameterType.GetInterfaces()
                                         .Any(i => i == typeof(IEnumerable)));
            var formatMethod = genericMethod.MakeGenericMethod(report.RowType, typeof(T));
            return formatMethod;
        }
    }
}