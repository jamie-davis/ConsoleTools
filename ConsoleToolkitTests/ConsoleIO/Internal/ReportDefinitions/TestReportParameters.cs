using System.Linq;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal.ReportDefinitions
{
    [TestFixture]
    public class TestReportParameters
    {
        #region Test types

        class TestRec
        {
            public int Integer { get; set; }
            public double Double { get; set; }
            public string String { get; set; }
        }

        #endregion
        [Test]
        public void ByDefaultOmitHeadingsIsFalse()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Assert
            Assert.That(parameters.Details.OmitHeadings, Is.False);
        }

        [Test]
        public void OmitHeadingsSettingIsRecorded()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Act
            parameters.OmitHeadings();

            //Assert
            Assert.That(parameters.Details.OmitHeadings, Is.True);
        }

        [Test]
        public void ByDefaultNoColumnsAreDefined()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Assert
            Assert.That(parameters.ColumnSource.Columns, Is.Empty);
        }

        [Test]
        public void AddedColumnsAreRecorded()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Act
            parameters.AddColumn(i => i.Integer, c => { });

            //Assert
            Assert.That(parameters.ColumnSource.Columns.Count(), Is.EqualTo(1));
        }

        [Test]
        public void AddedColumnsHaveCorrectType()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Act
            parameters.AddColumn(i => i.Integer, c => { });
            parameters.AddColumn(i => i.Double, c => { });
            parameters.AddColumn(i => i.String, c => { });

            //Assert
            var result = parameters.ColumnSource.Columns.Select(c => c.Type.Name).JoinWith(" ");
            Assert.That(result, Is.EqualTo("Int32 Double String"));
        }
    }
}
