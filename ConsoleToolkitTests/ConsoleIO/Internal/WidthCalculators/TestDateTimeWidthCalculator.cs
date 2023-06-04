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
    public class TestDateTimeWidthCalculator
    {
        public TestDateTimeWidthCalculator()
        {
            SetUpTests.OverrideCulture();
        }

        [Fact]
        public void DefaultFormatReturnsExpectedWidth()
        {
            var format = new ColumnFormat("A", typeof (DateTime));
            Assert.Equal(DateTime.Now.ToString().TrimEnd().Length, DateTimeWidthCalculator.Calculate(format));
        }

        [Fact]
        public void TemplatedFormatReturnsExpectedWidth()
        {
            const string template = "yyyy-MM-dd";
            var format = new ColumnFormat("A", typeof (DateTime), format: template);
            Assert.Equal(DateTime.Now.ToString(template).Length, DateTimeWidthCalculator.Calculate(format));
        }
    }
}