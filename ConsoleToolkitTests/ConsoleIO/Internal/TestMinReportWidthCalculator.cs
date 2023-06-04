using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestMinReportWidthCalculator
    {
        private static string[] _numbers = {"Zero", "One", "Two", "Three"};

        [Fact]
        public void MinWidthIsMinimumIdealWidthOfEachColumn()
        {
            var rep = Enumerable.Range(0, 3)
                .Select(i => new {Number = i, String = _numbers[i]});

            Assert.Equal(13, MinReportWidthCalculator.Calculate(rep, 1));
        }
    }
}
