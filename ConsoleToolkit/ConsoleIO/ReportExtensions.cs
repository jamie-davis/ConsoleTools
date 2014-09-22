using System;
using System.Collections.Generic;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;

namespace ConsoleToolkit.ConsoleIO
{
    public static class ReportExtensions
    {
        public static Report<T> AsReport<T>(this IEnumerable<T> items, Action<ReportParameters<T>> defineReport)
        {
            var reportParameters = new ReportParameters<T>();
            defineReport(reportParameters);
            return new Report<T>(items, reportParameters);
        }

    }
}