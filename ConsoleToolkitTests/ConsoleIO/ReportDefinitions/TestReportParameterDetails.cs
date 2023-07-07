using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    public class TestReportParameterDetails
    {
        [Fact]
        public void OmitHeadingsSetsReportOptions()
        {
            //Arrange
            var details = new ReportParameterDetails();

            //Act
            details.OmitHeadings = true;

            //Assert
            Assert.Equal(ReportFormattingOptions.OmitHeadings, details.Options);
        }

        [Fact]
        public void StretchColumnsSetsReportOptions()
        {
            //Arrange
            var details = new ReportParameterDetails();

            //Act
            details.StretchColumns = true;

            //Assert
            Assert.Equal(ReportFormattingOptions.StretchColumns, details.Options);
        }

        [Fact]
        public void SuppressHeadingRepetitionSetsReportOptions()
        {
            //Arrange
            var details = new ReportParameterDetails();

            //Act
            details.SuppressHeadingRepetition = true;

            //Assert
            Assert.Equal(ReportFormattingOptions.SuppressHeadingsAfterChildReport, details.Options);
        }

        [Fact]
        public void AllOptionsCanBeCombined()
        {
            //Arrange
            var details = new ReportParameterDetails();

            //Act
            details.OmitHeadings = true;
            details.StretchColumns = true;
            details.SuppressHeadingRepetition = true;

            //Assert
            Assert.Equal(ReportFormattingOptions.StretchColumns | ReportFormattingOptions.OmitHeadings | ReportFormattingOptions.SuppressHeadingsAfterChildReport, details.Options);
        }
    }
}
