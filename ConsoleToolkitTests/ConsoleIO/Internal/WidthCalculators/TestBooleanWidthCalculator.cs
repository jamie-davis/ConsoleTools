using System;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal.WidthCalculators;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal.WidthCalculators
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestBooleanWidthCalculator
    {
        [Test]
        public void DefaultFormatReturnsExpectedMaxWidth()
        {
            var format = new ColumnFormat("A", typeof (bool));
            Assert.That(BooleanWidthCalculator.Max(format), Is.EqualTo(Math.Max(true.ToString().Length, false.ToString().Length)));
        }

        [Test]
        public void DefaultFormatReturnsExpectedMinWidth()
        {
            var format = new ColumnFormat("A", typeof (bool));
            Assert.That(BooleanWidthCalculator.Min(format), Is.EqualTo(Math.Min(true.ToString().Length, false.ToString().Length)));
        }
    }
}