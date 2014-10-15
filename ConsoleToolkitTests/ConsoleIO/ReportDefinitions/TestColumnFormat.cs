using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    [TestFixture]
    public class TestColumnFormat
    {
        #region Test types

        class TestItem
        {
            public int Integer { get; set; }
        }

        #endregion

        [Test]
        public void DefaultColumnFormatHasNoHeading()
        {
            //Arrange
            var format = new ColumnConfig(null);
            format.MakeFormat<int>();

            //Assert
            Assert.That(format.ColumnFormat.Heading, Is.Null);
        }

        [Test]
        public void DefaultColumnFormatCorrectColumnType()
        {
            //Arrange
            var format = new ColumnConfig(null);
            format.MakeFormat<int>();

            //Assert
            Assert.That(format.ColumnFormat.Type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void DefaultColumnFormatHasNoFixedWidth()
        {
            //Arrange
            var format = new ColumnConfig(null);
            format.MakeFormat<int>();

            //Assert
            Assert.That(format.ColumnFormat.FixedWidth, Is.EqualTo(0));
        }
    }
}
