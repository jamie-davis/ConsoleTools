using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    [TestFixture]
    public class TestColumnConfig
    {
        #region Type for test

        class TestType
        {
            public int Value { get; set; }
        }

        #endregion

        private readonly Expression<Func<TestType, string>> _stringExp = t => t.Value.ToString();
        private readonly Expression<Func<TestType, int>> _intExp = t => t.Value;
        private readonly Expression<Func<TestType, double>> _doubleExp = t => t.Value/3.0;

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void HeadingCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.Heading("text");

            //Assert
            Assert.That(config.ColumnFormat.Heading, Is.EqualTo("text"));
        }

        [Test]
        public void LeftAlignmentCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_intExp);
            config.MakeFormat<int>();

            //Act
            config.LeftAlign();

            //Assert
            Assert.That(config.ColumnFormat.Alignment, Is.EqualTo(ColumnAlign.Left));
        }

        [Test]
        public void RightAlignmentCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.RightAlign();

            //Assert
            Assert.That(config.ColumnFormat.Alignment, Is.EqualTo(ColumnAlign.Right));
        }

        [Test]
        public void DecimalPlacesCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_doubleExp);
            config.MakeFormat<double>();

            //Act
            config.DecimalPlaces(6);

            //Assert
            Assert.That(config.ColumnFormat.DecimalPlaces, Is.EqualTo(6));
        }

        [Test]
        public void ColumnWidthCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.Width(6);

            //Assert
            Assert.That(config.ColumnFormat.FixedWidth, Is.EqualTo(6));
        }

        [Test]
        public void ColumnMinWidthCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.MinWidth(6);

            //Assert
            Assert.That(config.ColumnFormat.MinWidth, Is.EqualTo(6));
        }

        [Test]
        public void ColumnMaxWidthCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.MaxWidth(8);

            //Assert
            Assert.That(config.ColumnFormat.MaxWidth, Is.EqualTo(8));
        }
    }
}
