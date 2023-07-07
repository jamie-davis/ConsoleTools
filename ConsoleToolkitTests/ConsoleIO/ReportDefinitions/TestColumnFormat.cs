using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    public class TestColumnFormat
    {
        #region Test types

        class TestItem
        {
            public int Integer { get; set; }
        }

        #endregion

        [Fact]
        public void DefaultColumnFormatHasNoHeading()
        {
            //Arrange
            var format = new ColumnConfig(null);
            format.MakeFormat<int>();

            //Assert
            format.ColumnFormat.Heading.Should().BeNull();
        }

        [Fact]
        public void DefaultColumnFormatCorrectColumnType()
        {
            //Arrange
            var format = new ColumnConfig(null);
            format.MakeFormat<int>();

            //Assert
            Assert.Equal(typeof(int), format.ColumnFormat.Type);
        }

        [Fact]
        public void DefaultColumnFormatHasNoFixedWidth()
        {
            //Arrange
            var format = new ColumnConfig(null);
            format.MakeFormat<int>();

            //Assert
            Assert.Equal(0, format.ColumnFormat.FixedWidth);
        }
    }
}
