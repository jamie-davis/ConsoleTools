using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    public class TestProportionalColumnSizer
    {
        private ColumnSizingParameters _parameters;

        [SetUp]
        public void SetUp()
        {
            _parameters = new ColumnSizingParameters();
            _parameters.TabLength = 4;
            _parameters.SeperatorLength = 1;
        }

        private int[] GetWidths()
        {
            return _parameters.Columns.Select(c => c.Format.ActualWidth).ToArray();
        }

        private void SetCols(List<PropertyColumnFormat> formats)
        {
            _parameters.Columns = formats;
            _parameters.Sizers = _parameters.Columns.Select(f => new ColumnWidthNegotiator.ColumnSizerInfo(f, 4)).ToList();
        }

        [Test]
        public void EqualProportionsGenerateEqualWidths()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0})
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(30, 5, _parameters);

            //Assert
            var result = GetWidths();
            Assert.That(result, Is.EqualTo(new[] {5, 5, 5, 5, 5}));
        }

        [Test]
        public void ColumnsGetCorrectProportion()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 15.0}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(30, 5, _parameters);

            //Assert
            var result = GetWidths();
            Assert.That(result, Is.EqualTo(new[] {5, 5, 15}));
        }

        [Test]
        public void SpareCharacterIsAllocatedToWidestProportionalColumn()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0000001}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(31, 5, _parameters);

            //Assert
            var result = GetWidths();
            Assert.That(result, Is.EqualTo(new[] {5, 5, 5, 5, 6}));
        }

        [Test]
        public void ExtraCharactersAreSharedEqually()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0000001}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(34, 5, _parameters);

            //Assert
            var result = GetWidths();
            Assert.That(result, Is.EqualTo(new[] {6, 6, 6, 5, 6}));
        }

        [Test]
        public void LowProportionColumnLosesInSpareCharacterDistrbution()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0000001}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(34, 5, _parameters);

            //Assert
            var result = GetWidths();
            Assert.That(result, Is.EqualTo(new[] {5, 6, 6, 6, 6}));
        }

        [Test]
        public void AllColumnsGetAtLeastOneCharacter()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5000.0}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(34, 5, _parameters);

            //Assert
            var result = GetWidths();
            Console.WriteLine(string.Join(", ", result));
            Assert.That(result, Is.EqualTo(new[] {1, 1, 1, 1, 25}));
        }

        [Test]
        public void ColumnMinWidthLimitsShortestProportionalWidth()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001, MinWidth = 7}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5000.0}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(34, 5, _parameters);

            //Assert
            var result = GetWidths();
            Console.WriteLine(string.Join(", ", result));
            Assert.That(result, Is.EqualTo(new[] { 7, 1, 1, 1, 19 }));
        }

        [Test]
        public void MinWidthDoesNotIncreaseProportionalAllocation()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0, MinWidth = 3}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0, MinWidth = 3}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5.0}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(34, 5, _parameters);

            //Assert
            var result = GetWidths();
            Console.WriteLine(string.Join(", ", result));
            Assert.That(result, Is.EqualTo(new[] { 6, 6, 6, 6, 5 }));
        }

        [Test]
        public void ColumnMaxWidthLimitsLongestProportionalWidth()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5000.0, MaxWidth = 9}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(34, 5, _parameters);

            //Assert
            var result = GetWidths();
            Console.WriteLine(string.Join(", ", result));
            Assert.That(result, Is.EqualTo(new[] { 5, 5, 5, 5, 9 }));
        }

        [Test]
        public void ColumnMinAndMaxWidthLimitsAreAppliedTogether()
        {
            //Arrange
            var formats = new List<PropertyColumnFormat>
            {
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001, MinWidth = 7}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001, MinWidth = 7}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 0.0000001}),
                new PropertyColumnFormat(null, new ColumnFormat {ProportionalWidth = 5000.0, MaxWidth = 9}),
            };
            SetCols(formats);

            //Act
            ProportionalColumnSizer.Size(34, 5, _parameters);

            //Assert
            var result = GetWidths();
            Console.WriteLine(string.Join(", ", result));
            Assert.That(result, Is.EqualTo(new[] { 7, 7, 3, 3, 9 }));
        }
    }
}