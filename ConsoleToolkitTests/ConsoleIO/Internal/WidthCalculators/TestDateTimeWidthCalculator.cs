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
    public class TestDateTimeWidthCalculator
    {
        [SetUp]
        public void SetUp()
        {
            SetUpTests.OverrideCulture();
        }

        [Test]
        public void DefaultFormatReturnsExpectedWidth()
        {
            var format = new ColumnFormat("A", typeof (DateTime));
            Assert.That(DateTimeWidthCalculator.Calculate(format), Is.EqualTo(DateTime.Now.ToString().TrimEnd().Length));
        }

        [Test]
        public void TemplatedFormatReturnsExpectedWidth()
        {
            const string template = "yyyy-MM-dd";
            var format = new ColumnFormat("A", typeof (DateTime), format: template);
            Assert.That(DateTimeWidthCalculator.Calculate(format), Is.EqualTo(DateTime.Now.ToString(template).Length));
        }
    }
}