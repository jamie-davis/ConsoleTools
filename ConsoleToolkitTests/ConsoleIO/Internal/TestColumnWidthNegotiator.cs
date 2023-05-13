using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestColumnWidthNegotiator
    {
        private List<PropertyColumnFormat> _formats;

        private class TestType
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

        public TestColumnWidthNegotiator()
        {
            _formats = FormatAnalyser.Analyse(typeof (TestType), null, true);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void FixedWidthColumnsAreNeverStretched()
        {
            var longStringColFormat = _formats.First(f => f.Property.Name == "LongString").Format;
            longStringColFormat.FixedWidth = 4;
            longStringColFormat.SetActualWidth(4);
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

        [Fact]
        public void FixedWidthColumnsAreNeverShrunk()
        {
            var longStringColFormat = _formats.First(f => f.Property.Name == "LongString").Format;
            longStringColFormat.FixedWidth = 25;

            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAA"))
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

        [Fact]
        public void FixedWidthColumnsCanBeStacked()
        {
            var longStringColFormat = _formats.Last().Format;
            longStringColFormat.FixedWidth = 35;

            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(40);

            var output = cwn.StackedColumns.Select(sc => sc.Property.Name).JoinWith(", ");
            Assert.Equal("LongString", output);
        }

        [Fact]
        public void MinWidthColumnsAreNotShrunkPastMinimum()
        {
            var longStringColFormat = _formats.First(f => f.Property.Name == "Integer").Format;
            longStringColFormat.MinWidth = 9;

            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(30);

            var output = TabularReportRenderTool.Report(cwn, items);
            Approvals.Verify(output);
        }

        [Fact]
        public void MinWidthColumnsCanBeWiderThanMinimum()
        {
            var longStringColFormat = _formats.First(f => f.Property.Name == "LongString").Format;
            longStringColFormat.MinWidth = 9;

            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(30);

            var output = TabularReportRenderTool.Report(cwn, items);
            Approvals.Verify(output);
        }

        [Fact]
        public void ProportionalColumnsShareTheAvailableSpace()
        {
            var longStringColFormat = _formats.First(f => f.Property.Name == "LongString").Format;
            var shortStringColFormat = _formats.First(f => f.Property.Name == "ShortString").Format;
            longStringColFormat.ProportionalWidth = 1;
            shortStringColFormat.ProportionalWidth = 1;

            var cwn = new ColumnWidthNegotiator(_formats, 1);
            var items = Enumerable.Range(0, 5)
                .Select(i => new TestType("AAAA" + i, "AAAAAAA AAAAAAAA AAAA"))
                .ToList();

            cwn.AddHeadings();
            foreach (var item in items)
            {
                cwn.AddRow(item);
            }
            cwn.CalculateWidths(29);

            var output = TabularReportRenderTool.Report(cwn, items);
            Approvals.Verify(output);
        }
    }
}