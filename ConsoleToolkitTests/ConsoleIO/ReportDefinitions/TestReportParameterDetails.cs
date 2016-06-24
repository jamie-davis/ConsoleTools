using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    [TestFixture]
    public class TestReportParameterDetails
    {
        [Test]
        public void OmitHeadingsSetsReportOptions()
        {
            //Arrange
            var details = new ReportParameterDetails();

            //Act
            details.OmitHeadings = true;

            //Assert
            Assert.That(details.Options, Is.EqualTo(ReportFormattingOptions.OmitHeadings));
        }

        [Test]
        public void StretchColumnsSetsReportOptions()
        {
            //Arrange
            var details = new ReportParameterDetails();

            //Act
            details.StretchColumns = true;

            //Assert
            Assert.That(details.Options, Is.EqualTo(ReportFormattingOptions.StretchColumns));
        }

        [Test]
        public void SuppressHeadingRepetitionSetsReportOptions()
        {
            //Arrange
            var details = new ReportParameterDetails();

            //Act
            details.SuppressHeadingRepetition = true;

            //Assert
            Assert.That(details.Options, Is.EqualTo(ReportFormattingOptions.SuppressHeadingsAfterChildReport));
        }

        [Test]
        public void AllOptionsCanBeCombined()
        {
            //Arrange
            var details = new ReportParameterDetails();

            //Act
            details.OmitHeadings = true;
            details.StretchColumns = true;
            details.SuppressHeadingRepetition = true;

            //Assert
            Assert.That(details.Options, Is.EqualTo(ReportFormattingOptions.StretchColumns 
                                                    | ReportFormattingOptions.OmitHeadings 
                                                    | ReportFormattingOptions.SuppressHeadingsAfterChildReport));
        }
    }
}
