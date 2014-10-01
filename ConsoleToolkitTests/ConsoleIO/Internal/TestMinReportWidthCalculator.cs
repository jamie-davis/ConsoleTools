using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestMinReportWidthCalculator
    {
        private static string[] _numbers = {"Zero", "One", "Two", "Three"};

        [Test]
        public void MinWidthIsMinimumIdealWidthOfEachColumn()
        {
            var rep = Enumerable.Range(0, 3)
                .Select(i => new {Number = i, String = _numbers[i]});

            Assert.That(MinReportWidthCalculator.Calculate(rep, 1), Is.EqualTo(13));
        }
    }
}
