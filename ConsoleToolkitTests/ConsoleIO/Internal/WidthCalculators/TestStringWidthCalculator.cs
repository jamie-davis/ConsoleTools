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
    public class TestStringWidthCalculator
    {
        [Fact]
        public void DefaultFormatReturnsExpectedMinWidth()
        {
            var format = new ColumnFormat("A", typeof (string));
            Assert.Equal(string.Empty.Length, StringWidthCalculator.Min(format));
        }

        [Fact]
        public void TemplatedFormatReturnsExpectedMinWidth()
        {
            const string template = @"String: {0}";
            var format = new ColumnFormat("A", typeof (TimeSpan), format: template);
            Assert.Equal(string.Empty.Length, StringWidthCalculator.Min(format));
        }
    }
}