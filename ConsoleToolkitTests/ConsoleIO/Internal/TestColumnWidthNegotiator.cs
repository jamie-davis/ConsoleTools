using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestColumnWidthNegotiator
    {
        private List<PropertyColumnFormat> _formats;

        class TestType
        {
            public string ShortString { get; set; }
            public int Integer { get; set; }
            public string LongString { get; set; }

            public TestType(string shortString, string longString)
            {
                ShortString = shortString;
                LongString = longString;
                Integer = shortString.Length + longString.Length;
            }
        }

        [SetUp]
        public void TestFixtureSetUp()
        {
            _formats = FormatAnalyser.Analyse(typeof(TestType), null, true);
        }

        [Test]
        public void ColumnsArePreciselySizedWhenDataFits()
        {
            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAAAAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(80);

            var output = TabularReportRenderTool.Report(cwn, items);
            Approvals.Verify(output);
        }

        [Test]
        public void RowDataCanBeLoadedFromCachedRow()
        {
            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAAAAAA"))
                .ToList();
            var cachedData = new CachedRows<TestType>(items);

            cwn.AddHeadings();
            foreach (var row in cachedData.GetRows())
            {
                cwn.AddRow(row);
            }
            cwn.CalculateWidths(80);

            var output = TabularReportRenderTool.Report(cwn, items);
            Approvals.Verify(output);
        }

        [Test]
        public void ColumnsAreShrunkInOrderToFitData()
        {
            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAAAAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(40);

            var output = TabularReportRenderTool.Report(cwn, items);
            Approvals.Verify(output);
        }

        [Test]
        public void IfTheReportCannotBeShrunkTheRightmostColumnsAreStacked()
        {
            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAAAAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(20);

            var output = TabularReportRenderTool.Report(cwn, items);
            Approvals.Verify(output);
        }

        [Test]
        public void SizingDataCanBeRetrievedFromSizedColumns()
        {
            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAAAAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(80);

            var output = TabularReportRenderTool.ReportSizingData(cwn);
            Approvals.Verify(output);
        }

        [Test]
        public void SizingDataIncludesStackedProperties()
        {
            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAAAAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(20);

            var output = TabularReportRenderTool.ReportSizingData(cwn);
            Console.WriteLine(output);
            Approvals.Verify(output);
        }
    }
}