using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestColumnExpander
    {
        private ColumnSizingParameters _parameters;
        private List<TestType> _data;
        private int _seperatorOverhead;
        private string _initialReport;

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        class TestType
        {
            public string ShortString { get; set; }
            public int Integer { get; set; }
            public string LongString { get; set; }
            public string SecondLongString { get; set; }

            public TestType(string shortString, string longString, string secondLongString = null)
            {
                ShortString = shortString;
                SecondLongString = secondLongString ?? longString;
                LongString = longString;
                Integer = shortString.Length + longString.Length;
            }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore MemberCanBePrivate.Local

        [SetUp]
        public void SetUp()
        {
            _data = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAAAAAA"))
                .ToList();

            _parameters = new ColumnSizingParameters();
            _parameters.Columns = FormatAnalyser.Analyse(typeof(TestType), null, true);
            _parameters.Sizers = _parameters.Columns.Select(f => new ColumnWidthNegotiator.ColumnSizerInfo(f, 4)).ToList();
            _parameters.TabLength = 4;
            _parameters.SeperatorLength = 1;

            ImportSizeData();

            _seperatorOverhead = 3;
            const int defaultWidth = 26;
            ColumnShrinker.ShrinkColumns(defaultWidth, _seperatorOverhead, _parameters);

            CreateInitialReport(defaultWidth);
        }

        private void CreateInitialReport(int width)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Initial report:");
            sb.AppendLine(RulerFormatter.MakeRuler(width));
            sb.AppendLine(TabularReportRenderTool.Report(_parameters, _data));
            sb.AppendLine();
            sb.AppendLine("Expanded report:");

            _initialReport = sb.ToString();
        }

        private void ImportSizeData()
        {
            foreach (var testType in _data)
            {
                foreach (var sizer in _parameters.Sizers)
                {
                    var value = sizer.PropertyColumnFormat.Property.GetValue(testType, null);
                    sizer.Sizer.ColumnValue(value);
                }
            }
        }

        [Test]
        public void ReportWithNoWrappingIsExpanded()
        {
            const int width = 80;
            ColumnExpander.FillAvailableSpace(width, _seperatorOverhead, _parameters, true);

            var ruler = RulerFormatter.MakeRuler(width) + Environment.NewLine;
            var report = TabularReportRenderTool.Report(_parameters, _data);
            Approvals.Verify(_initialReport + ruler + report);
        }

        [Test]
        public void ReportWithWrappingIsExpanded()
        {
            const int width = 43;
            ColumnExpander.FillAvailableSpace(width, _seperatorOverhead, _parameters, true);

            var ruler = RulerFormatter.MakeRuler(width) + Environment.NewLine;
            var report = TabularReportRenderTool.Report(_parameters, _data);
            Approvals.Verify(_initialReport + ruler + report);
        }

        [Test]
        public void FixedWidthColumnsAreNotExpanded()
        {
            const int width = 43;
            _parameters.Columns.First(c => c.Format.Heading == "Long String").Format.FixedWidth = 4;
            ColumnExpander.FillAvailableSpace(width, _seperatorOverhead, _parameters, true);

            var ruler = RulerFormatter.MakeRuler(width) + Environment.NewLine;
            var report = TabularReportRenderTool.Report(_parameters, _data);
            Approvals.Verify(_initialReport + ruler + report);
        }
    }
}