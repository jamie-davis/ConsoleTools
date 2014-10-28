using System;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Internal.WidthCalculators;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal.WidthCalculators
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestTimeSpanWidthCalculator
    {
        private TimeSpan _testValue;

        [SetUp]
        public void SetUp()
        {
            _testValue = TimeSpan.MinValue;
        }

        [Test]
        public void DefaultFormatReturnsExpectedWidth()
        {
            var format = new ColumnFormat("A", typeof (TimeSpan));
            Assert.That(TimeSpanWidthCalculator.Calculate(format), Is.EqualTo(_testValue.ToString().Length));
        }

        [Test]
        public void TemplatedFormatReturnsExpectedWidth()
        {
            const string template = @"hh\:mm\:ss";
            var format = new ColumnFormat("A", typeof (TimeSpan), format: template);
            Assert.That(TimeSpanWidthCalculator.Calculate(format), Is.EqualTo(_testValue.ToString(template).Length));
        }
    }
}