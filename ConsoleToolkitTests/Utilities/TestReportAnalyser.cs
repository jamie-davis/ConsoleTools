using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using ConsoleToolkit.Utilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.Utilities
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestReportAnalyser
    {
        private Tuple<int, string>[] _data;
        private Report<Tuple<int, string>> _report;
        private UnitTestConsole _utc;

        [SetUp]
        public void SetUp()
        {
            _data = Enumerable.Range(0, 10)
                .Select(x => Tuple.Create(x, string.Format("Num = {0}", x)))
                .ToArray();
            _report = _data.AsReport(rep => rep
                .AddColumn(r => r.Item1, cc => cc.Heading("Number"))
                .AddColumn(r => r.Item2, cc => cc.Heading("Text"))
                );

        _utc = new UnitTestConsole();
            _utc.Interface.BufferWidth = 133;
            _utc.Interface.WindowWidth = 133;
        }

        [Test]
        public void ColumnsAreAvailable()
        {
            //Arrange
            var analysis = ReportAnalyser.Analyse(_report);

            //Act
            var columns = string.Join(", ", analysis.Columns);

            //Assert
            Assert.That(columns, Is.EqualTo("Number, Text"));
        }

        [Test]
        public void ColumnValuesFromEachRowAreAvailable()
        {
            //Arrange
            var analysis = ReportAnalyser.Analyse(_report);

            //Act
            var rowData = GetAllRows(analysis).Select((s, n) => new {Row = n, Values = s});

            //Assert
            _utc.Console.FormatTable(rowData);
            Approvals.Verify(_utc.Interface.GetBuffer());
        }

        private IEnumerable<string> GetAllRows<T>(ReportAnalysis<T> analysis)
        {
            while (analysis.MoveNext())
            {
                yield return string.Join(", ", analysis.Columns.Select(analysis.GetColumnValue));
            }
        }
    }
}
