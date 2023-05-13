using System.Linq;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal.ReportDefinitions
{
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
        [Fact]
        public void ByDefaultOmitHeadingsIsFalse()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Assert
            parameters.Details.OmitHeadings.Should().BeFalse();
        }

        [Fact]
        public void OmitHeadingsSettingIsRecorded()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Act
            parameters.OmitHeadings();

            //Assert
            parameters.Details.OmitHeadings.Should().BeTrue();
        }

        [Fact]
        public void IndentIsZeroByDefault()
        {
            //Act
            var parameters = new ReportParameters<TestRec>();

            //Assert
            Assert.Equal(0, parameters.Details.IndentSpaces);
        }

        [Fact]
        public void RequestedIndentIsRecorded()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Act
            parameters.Indent(4);

            //Assert
            Assert.Equal(4, parameters.Details.IndentSpaces);
        }

        [Fact]
        public void ByDefaultNoColumnsAreDefined()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Assert
            parameters.ColumnSource.Columns.Should().BeEmpty();
        }

        [Fact]
        public void AddedColumnsAreRecorded()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Act
            parameters.AddColumn(i => i.Integer, c => { });

            //Assert
            Assert.Equal(1, parameters.ColumnSource.Columns.Count());
        }

        [Fact]
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
            Assert.Equal("Int32 Double String", result);
        }

        [Fact]
        public void SuppressHeadingsSettingIsRecorded()
        {
            //Arrange
            var parameters = new ReportParameters<TestRec>();

            //Act
            parameters.SuppressHeadingRepetition();

            //Assert
            parameters.Details.SuppressHeadingRepetition.Should().BeTrue();
        }
    }
}
