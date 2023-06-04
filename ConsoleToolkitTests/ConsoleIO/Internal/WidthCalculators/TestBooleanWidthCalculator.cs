using System;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Internal.WidthCalculators;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal.WidthCalculators
{
    [UseReporter(typeof (CustomReporter))]
    public class TestBooleanWidthCalculator
    {
        [Fact]
        public void DefaultFormatReturnsExpectedMaxWidth()
        {
            var format = new ColumnFormat("A", typeof (bool));
            Assert.Equal(Math.Max(true.ToString().Length, false.ToString().Length), BooleanWidthCalculator.Max(format));
        }

        [Fact]
        public void DefaultFormatReturnsExpectedMinWidth()
        {
            var format = new ColumnFormat("A", typeof (bool));
            Assert.Equal(Math.Min(true.ToString().Length, false.ToString().Length), BooleanWidthCalculator.Min(format));
        }
    }
}