using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using ConsoleToolkit.Utilities;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestTabularReport
    {
        [SetUp]
        public void SetUp()
        {
            SetUpTests.OverrideCulture();
        }

        [Test]
        public void TabularReportWithoutWrappingIsFormatted()
        {
            SetUpTests.OverrideCulture();
            var data = Enumerable.Range(0, 10)
                .Select(i => new
                {
                    String = string.Format("String value {0}", i),
                    Int = i,
                    Double = 3.0/(i + 1.0),
                    DateTime = DateTime.Parse(string.Format("2014-{0}-17", i+1))
                })
                .ToList();

            var report = Report(data, options: ReportFormattingOptions.StretchColumns);
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
        public void ColumnHeadingsCanBeOmitted()
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

            var report = Report(data, options: ReportFormattingOptions.Default | ReportFormattingOptions.OmitHeadings);
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
        public void TabularReportCanFormatFromCachedData()
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

            var dataCache = CachedRowsFactory.Make(data);

            var report = CachedReport(dataCache);
            Approvals.Verify(report);
        }

        [Test]
        public void HeadingsCanBeOmittedFromCachedData()
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

            var dataCache = CachedRowsFactory.Make(data);

            var report = CachedReport(dataCache, options: ReportFormattingOptions.Default | ReportFormattingOptions.OmitHeadings);
            Approvals.Verify(report);
        }

        [Test]
        public void TabularReportCanFormatRenderableData()
        {
            var data = Enumerable.Range(3, 7)
                .Select(i => new
                {
                    String = MakeRenderable(i),
                    SecondString = string.Format("Second long string value {0}. {1}", i, string.Join(" ", Enumerable.Repeat("variation ", i))),
                    Int = i,
                    Double = 3.0/(i + 1.0),
                    DateTime = DateTime.Parse(string.Format("2014-{0}-17", i+1))
                })
                .ToList();

            var dataCache = CachedRowsFactory.Make(data);

            var report = CachedReport(dataCache, options: ReportFormattingOptions.StretchColumns);
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

        [Test]
        public void ColumnsCanBeDividedWithCustomText()
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

            sb.Append(Report(data, 50, columnDivider: "XXX"));
            sb.AppendLine();

            Approvals.Verify(sb.ToString());
        }

        [Test]
        public void ColumnWidthsDoNotTakeHeadingIntoAccountIfHeadingsSkipped()
        {
            var data = Enumerable.Range(0, 1)
                .Select(i => new
                {
                    Value = "X",
                    Text = "Text.",
                })
                .ToList();

            var sb = new StringBuilder();

            sb.AppendLine("With headings:");
            sb.AppendLine();
            sb.Append(Report(data, 50));
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Without headings:");
            sb.AppendLine();

            sb.Append(Report(data, 50, options: ReportFormattingOptions.OmitHeadings));
            sb.AppendLine();

            Approvals.Verify(sb.ToString());
        }

        [Test]
        public void AChildReportsIsRendered()
        {
            var data = Enumerable.Range(0, 1)
                .AsReport(p => p.AddColumn(r => "X", cc => cc.Heading("Value"))
                                .AddColumn(r => "Text", cc => cc.Heading("Text."))
                                .AddChild(i => Enumerable.Range(0,2), cr => cr.AddColumn(i => i, cc => cc.Heading("Child Int"))));

            var sb = new StringBuilder();

            sb.Append(Report(data, 50));

            Approvals.Verify(sb.ToString());
        }

        [Test]
        public void APrimitiveProducesAOneColumnReport()
        {
            var data = Enumerable.Range(0, 15);

            var sb = new StringBuilder();

            sb.Append(Report(data, 50, 10));

            Approvals.Verify(sb.ToString());
        }

        [Test]
        public void APrimitiveOneColumnReportCanHaveAColumnFormat()
        {
            var data = Enumerable.Range(0, 15)
                .AsReport(p => p.AddColumn(r => r, cc => cc.Heading("Int Value").LeftAlign()));

            var sb = new StringBuilder();

            sb.Append(Report(data, 50, 10));

            Approvals.Verify(sb.ToString());
        }

        private RecordingConsoleAdapter MakeRenderable(int number, bool narrow = false)
        {
            var output = new RecordingConsoleAdapter();
            if (narrow)
                output.WrapLine("Narrow text");
            else
                output.WrapLine("Some wrapped text containing the index number :{0} and some more stuff.", number);

            var table = Enumerable.Range(0, number)
                .Select(i => new { Text = string.Format("Nested row {0}", i)});
            output.FormatTable(table, ReportFormattingOptions.StretchColumns);

            return output;
        }

        private static string Report<T>(IEnumerable<T> data, int width = 80, int numRowsToUseForSizing = 0, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnDivider = null)
        {
            var report = RulerFormatter.MakeRuler(width)
                         + Environment.NewLine
                         + string.Join(string.Empty, TabularReport.Format<T, T>(data, null, width, numRowsToUseForSizing, options, columnDivider));
            Console.WriteLine(report);
            return report;
        }

        private static string Report<T>(Report<T> report, int width = 80, int numRowsToUseForSizing = 0)
        {
            var formatMethod = MakeFormatMethodInfo(report);
            var parameters = new object[]
                                 {
                                     report.Query,
                                     report.Columns,
                                     width,
                                     numRowsToUseForSizing, //rows to use for sizing
                                     report.Options,
                                     report.ColumnDivider,
                                     report.Children
                                 };

            var tabular = MethodInvoker.Invoke(formatMethod, null, parameters) as IEnumerable<string>;
            var output = RulerFormatter.MakeRuler(width)
                         + Environment.NewLine
                         + string.Join(string.Empty, tabular);
            return output;
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

        private static string CachedReport<T>(CachedRows<T> data, int width = 80, ReportFormattingOptions options = ReportFormattingOptions.Default | ReportFormattingOptions.IncludeAllColumns, string columnDivider = null)
        {
            var report = RulerFormatter.MakeRuler(width)
                         + Environment.NewLine
                         + string.Join(string.Empty, TabularReport.Format(data, null, width, options, columnDivider));
            Console.WriteLine(report);
            return report;
        }
    }
}