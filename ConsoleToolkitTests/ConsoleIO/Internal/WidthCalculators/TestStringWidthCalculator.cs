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
    public class TestStringWidthCalculator
    {
        [Test]
        public void DefaultFormatReturnsExpectedMinWidth()
        {
            var format = new ColumnFormat("A", typeof (string));
            Assert.That(StringWidthCalculator.Min(format), Is.EqualTo(string.Empty.Length));
        }

        [Test]
        public void TemplatedFormatReturnsExpectedMinWidth()
        {
            const string template = @"String: {0}";
            var format = new ColumnFormat("A", typeof (TimeSpan), format: template);
            Assert.That(StringWidthCalculator.Min(format), Is.EqualTo(string.Empty.Length));
        }
    }
}