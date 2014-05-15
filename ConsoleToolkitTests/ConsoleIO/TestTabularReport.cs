using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestTabularReport
    {
        [Test]
        public void TabularReportWithoutWrappingIsFormatted()
        {
            var data = Enumerable.Range(0, 10)
                .Select(i => new
                {
                    String = string.Format("String value {0}", i),
                    Int = i,
                    Double = 3.0/(i + 1.0),
                    DateTime = DateTime.Parse(string.Format("2014-{0}-17", i+1))
                })
                .ToList();

            var report = Report(data);
            Approvals.Verify(report);
        }

        [Test]
        public void TabularReportWithWrappingIsFormatted()
        {
            var data = Enumerable.Range(0, 10)
                .Select(i => new
                {
                    String = string.Format("Long string value {0}. This value should be nice and long so that it doesn't fit a line. Then the algorithm has to calculate how many lines it will take to display the value within the allowable space. Better add a bit of variation to each row too: {1}", i, string.Join(" ", Enumerable.Repeat("variation ", i))),
                    Int = i,
                    Double = 3.0/(i + 1.0),
                    DateTime = DateTime.Parse(string.Format("2014-{0}-17", i+1))
                })
                .ToList();

            var report = Report(data);
            Approvals.Verify(report);
        }

        [Test]
        public void TabularReportWithTwoWrappingTextColumnsIsFormatted()
        {
            var data = Enumerable.Range(0, 10)
                .Select(i => new
                {
                    String = string.Format("First long string value {0}. {1}", i, string.Join(" ", Enumerable.Repeat("variation ", i))),
                    SecondString = string.Format("Second long string value {0}. {1}", i, string.Join(" ", Enumerable.Repeat("variation ", i))),
                    Int = i,
                    Double = 3.0/(i + 1.0),
                    DateTime = DateTime.Parse(string.Format("2014-{0}-17", i+1))
                })
                .ToList();

            var report = Report(data);
            Approvals.Verify(report);
        }

        [Test]
        public void IdealMinimumWidthIsAppliedToStringColumns()
        {
            var data = Enumerable.Range(0, 1)
                .Select(i => new
                {
                    S = "X X X Y",
                    S2 = "X X X YY",
                    S3 = "XXX XXX XXX YYY",
                    S4 = "XXX XXX XXX XXX XXX YYYY",
                    S5 = "XXX XXX XXX YYYYY",
                })
                .ToList();

            var report = Report(data, 19);
            Approvals.Verify(report);
        }

        [Test]
        public void LastColumnIsStackedWhenColumnsDoNotFit()
        {
            var data = Enumerable.Range(0, 1)
                .Select(i => new
                {
                    S = "X X X Y",
                    S2 = "X X X YY",
                    S3 = "XXX XXX XXX YYY",
                    S4 = "XXX XXX XXX XXX XXX YYYY",
                    S5 = "XXX XXX XXX YYYYY",
                })
                .ToList();

            var report = Report(data, 17);
            Approvals.Verify(report);
        }

        [Test]
        public void AllColumnsCanBeStacked()
        {
            var data = Enumerable.Range(0, 1)
                .Select(i => new
                {
                    Value = 1.5,
                    LongText = "Long text that could be wrapped quite easily if required.",
                    Short = "Short text",
                    Date = DateTime.Parse("2014-05-07 19:59:20"),
                })
                .ToList();

            var report = Report(data, 20);
            Approvals.Verify(report);
        }

        [Test]
        public void FormattingSucceedsWhenAvailableSpaceIsTooSmall()
        {
            var data = Enumerable.Range(0, 1)
                .Select(i => new
                {
                    Value = 1.5,
                    LongText = "Long text that could be wrapped quite easily if required.",
                    Short = "Short text",
                    Date = DateTime.Parse("2014-05-07 19:59:20"),
                })
                .ToList();

            var report = Report(data, 4);
            Approvals.Verify(report);
        }

        [Test]
        public void SizesAreOnlyCalculatedFromSpecifiedRows()
        {
            var data = Enumerable.Range(0, 20)
                .Select(i => new
                {
                    Value = 1.5,
                    X = i
                })
                .ToList();

            var report = Report(data, 9, 9);
            Approvals.Verify(report);
        }

        [Test]
        public void ReportDegradesGracefully()
        {
            var data = Enumerable.Range(0, 1)
                .Select(i => new
                {
                    Value = 1.5,
                    LongText = "Long text that could be wrapped quite easily if required.",
                    Short = "Short text",
                    Date = DateTime.Parse("2014-05-07 19:59:20"),
                })
                .ToList();

            var sb = new StringBuilder();

            for (var width = 80; width > 0; width -= 5)
            {
                sb.AppendLine(string.Format("Test width {0}:", width));
                sb.Append(Report(data, width));
                sb.AppendLine();
            }
            Approvals.Verify(sb.ToString());
        }

        private static string Report<T>(IEnumerable<T> data, int width = 80, int numRowsToUseForSizing = 0)
        {
            var report = RulerFormatter.MakeRuler(width)
                         + Environment.NewLine
                         + string.Join(string.Empty, TabularReport.Format(data, null, width, numRowsToUseForSizing));
            Console.WriteLine(report);
            return report;
        }
    }
}