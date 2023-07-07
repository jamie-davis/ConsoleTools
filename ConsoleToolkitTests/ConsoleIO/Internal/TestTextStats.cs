using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestTextStats
    {
        [Fact]
        public void MaximumLengthIsMeasured()
        {
            var stats = new TextStats();
            ProcessNumberStrings(stats);

            Assert.Equal(5, stats.MaxWidth);
        }

        [Fact]
        public void MinimumLengthIsMeasured()
        {
            var stats = new TextStats();
            ProcessNumberStrings(stats);

            Assert.Equal(3, stats.MinWidth);
        }

        [Fact]
        public void MinimumLengthCanBeZero()
        {
            var stats = new TextStats();
            stats.Add(string.Empty);
            ProcessNumberStrings(stats);

            Assert.Equal(0, stats.MinWidth);
        }

        [Fact]
        public void AverageLengthIsMeasured()
        {
            var stats = new TextStats();

            stats.Add(new string('*',100));
            stats.Add(new string('*',50));

            Assert.Equal(75.0, stats.AvgWidth);
        }

        [Fact]
        public void LengthTalliesAreMaintained()
        {
            var stats = new TextStats();

            ProcessNumberStrings(stats);

            var tallies = stats.LengthTallies.Select(t => string.Format("[{0} = {1}]", t.Length, t.Count)).Aggregate((t, i) => t + i);

            Assert.Equal("[3 = 4][5 = 3][4 = 3]", tallies);
        }

        private static void ProcessNumberStrings(TextStats stats)
        {
            var values = new[] {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"};
            foreach (var value in values)
            {
                stats.Add(value);
            }
        }
    }
}
