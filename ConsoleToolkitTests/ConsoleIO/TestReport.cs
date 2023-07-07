using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO
{
    [UseReporter(typeof (CustomReporter))]
    public class TestReport
    {
        private ConsoleInterfaceForTesting _consoleInterface;
        private ConsoleAdapter _adapter;
        private string _longText;
        public TestReport()
        {
            _longText = "ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ";

            _consoleInterface = new ConsoleInterfaceForTesting();
            _consoleInterface.BufferWidth = 80;
            _consoleInterface.WindowWidth = 80;

            _adapter = new ConsoleAdapter(_consoleInterface);
        }

        [Fact]
        public void AReportIsGenerated()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign()));

            //Assert
            Assert.Equal(1, report.Columns.Count());
        }

        [Fact]
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

        [Fact]
        public void TheOmitHeadingsOptionSetsTheReportOptions()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .OmitHeadings());

            //Assert
            Assert.Equal(ReportFormattingOptions.OmitHeadings, report.Options);
        }

        [Fact]
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

        [Fact]
        public void TheStretchColumnsOptionSetsTheReportOptions()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .StretchColumns());

            //Assert
            Assert.Equal(ReportFormattingOptions.StretchColumns, report.Options);
        }

        [Fact]
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

        [Fact]
        public void TheReportCanBeIndented()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var indentReport = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .Indent(4)
                                                 .StretchColumns());
            var normalReport = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .StretchColumns());

            //Act
            _adapter.FormatTable(indentReport);
            _adapter.FormatTable(normalReport);

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void ReportWithChildReportCanBeIndented()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var indentReport = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .AddChild(r => Enumerable.Range(0,2), innerRep => innerRep.AddColumn(n => n, cc => cc.Heading("Number")))
                                                 .Indent(4)
                                                 .StretchColumns());
            var normalReport = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .AddChild(r => Enumerable.Range(0, 2), innerRep => innerRep.AddColumn(n => n, cc => cc.Heading("Number")))
                                                 .StretchColumns());

            //Act
            _adapter.FormatTable(indentReport);
            _adapter.FormatTable(normalReport);

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void ColourChildReportCanBeIndented()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var indentReport = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .AddChild(r => Enumerable.Range(0,1), 
                                                    innerRep => innerRep
                                                        .AddColumn(n => "111 " + _longText.Red(), cc => cc.Heading("Text1"))
                                                        .AddColumn(n => "22 " + _longText.Blue(), cc => cc.Heading("Text2"))
                                                        )
                                                 .Indent(4)
                                                 .StretchColumns());

            //Act
            _adapter.FormatTable(indentReport);

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Fact]
        public void ReportTitleIsPrinted()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .Indent(4)
                                                 .StretchColumns()
                                                 .Title("Test".Red() + " " + "Report".Green()));

            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void ReportTitleIsWrapped()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .Indent(4)
                                                 .StretchColumns()
                                                 .Title(_longText));

            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void AllOptionsCanBeSetAtOnce()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.StretchColumns()
                                                 .OmitHeadings()
                                                 .AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .Indent(4)
                                                 );

            //Assert
            var expected = ReportFormattingOptions.StretchColumns | ReportFormattingOptions.OmitHeadings;
            Assert.Equal(expected, report.Options);
        }

        [Fact]
        public void ChildReportsCanBeAdded()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.Heading("Test values"))
                                                 .AddChild(n => Enumerable.Range(1, 4),
                                                           p => p.AddColumn(i => i, c => c.Heading("Nested Number")))
                );

            //Assert
            _adapter.FormatTable(report);
            Approvals.Verify(_consoleInterface.GetBuffer());
        }
    }
}
