using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Testing;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestReport
    {
        private ConsoleInterfaceForTesting _consoleInterface;
        private ConsoleAdapter _adapter;

        [SetUp]
        public void SetUp()
        {
            _consoleInterface = new ConsoleInterfaceForTesting();
            _consoleInterface.BufferWidth = 80;
            _consoleInterface.WindowWidth = 80;

            _adapter = new ConsoleAdapter(_consoleInterface);
        }

        [Test]
        public void AReportIsGenerated()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign()));

            //Assert
            Assert.That(report.Columns.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ReportIsDisplayedWithDefaultOptions()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data
                .Select(n => new {TestValue = string.Format("Test value {0}", n)})
                .AsReport(rep => rep.AddColumn(n => n.TestValue,
                                               col => col.RightAlign()));
            
            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void TheOmitHeadingsOptionSetsTheReportOptions()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .OmitHeadings());

            //Assert
            Assert.That(report.Options, Is.EqualTo(ReportFormattingOptions.OmitHeadings));
        }

        [Test]
        public void TheOmitHeadingsOptionIsApplied()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .OmitHeadings());

            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void TheStretchColumnsOptionSetsTheReportOptions()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .StretchColumns());

            //Assert
            Assert.That(report.Options, Is.EqualTo(ReportFormattingOptions.StretchColumns));
        }

        [Test]
        public void TheStretchColumnsOptionIsApplied()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .StretchColumns());

            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void AllOptionsCanBeSetAtOnce()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.StretchColumns()
                                                 .OmitHeadings()
                                                 .AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 );

            //Assert
            var expected = ReportFormattingOptions.StretchColumns | ReportFormattingOptions.OmitHeadings;
            Assert.That(report.Options, Is.EqualTo(expected));
        }
    }
}
