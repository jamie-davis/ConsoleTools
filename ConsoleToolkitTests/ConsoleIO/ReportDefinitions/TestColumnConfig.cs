using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
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
        public TestColumnConfig()
        {
        }

        [Fact]
        public void HeadingCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.Heading("text");

            //Assert
            Assert.Equal("text", config.ColumnFormat.Heading);
        }

        [Fact]
        public void LeftAlignmentCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_intExp);
            config.MakeFormat<int>();

            //Act
            config.LeftAlign();

            //Assert
            Assert.Equal(ColumnAlign.Left, config.ColumnFormat.Alignment);
        }

        [Fact]
        public void RightAlignmentCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.RightAlign();

            //Assert
            Assert.Equal(ColumnAlign.Right, config.ColumnFormat.Alignment);
        }

        [Fact]
        public void DecimalPlacesCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_doubleExp);
            config.MakeFormat<double>();

            //Act
            config.DecimalPlaces(6);

            //Assert
            Assert.Equal(6, config.ColumnFormat.DecimalPlaces);
        }

        [Fact]
        public void ColumnWidthCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.Width(6);

            //Assert
            Assert.Equal(6, config.ColumnFormat.FixedWidth);
        }

        [Fact]
        public void ColumnMinWidthCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.MinWidth(6);

            //Assert
            Assert.Equal(6, config.ColumnFormat.MinWidth);
        }

        [Fact]
        public void ColumnMaxWidthCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.MaxWidth(8);

            //Assert
            Assert.Equal(8, config.ColumnFormat.MaxWidth);
        }

        [Fact]
        public void ColumnProportionalWidthCanBeSet()
        {
            //Arrange
            var config = new ColumnConfig(_stringExp);
            config.MakeFormat<string>();

            //Act
            config.ProportionalWidth(8.4);

            //Assert
            Assert.Equal(8.4, config.ColumnFormat.ProportionalWidth);
        }
    }
}
