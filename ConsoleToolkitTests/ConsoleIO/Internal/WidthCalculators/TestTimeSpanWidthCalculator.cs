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
    public class TestTimeSpanWidthCalculator
    {
        private TimeSpan _testValue;
        public TestTimeSpanWidthCalculator()
        {
            _testValue = TimeSpan.MinValue;
        }

        [Fact]
        public void DefaultFormatReturnsExpectedWidth()
        {
            var format = new ColumnFormat("A", typeof (TimeSpan));
            Assert.Equal(_testValue.ToString().Length, TimeSpanWidthCalculator.Calculate(format));
        }

        [Fact]
        public void TemplatedFormatReturnsExpectedWidth()
        {
            const string template = @"hh\:mm\:ss";
            var format = new ColumnFormat("A", typeof (TimeSpan), format: template);
            Assert.Equal(_testValue.ToString(template).Length, TimeSpanWidthCalculator.Calculate(format));
        }
    }
}