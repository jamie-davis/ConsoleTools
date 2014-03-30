using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.Utilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.Utilities
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestTextFormatter
    {
        [Test]
        public void BlocksWithEqualLengthAreMergedProperly()
        {
            var left =
                @"First line
Second line";
            var right =
                @"A
B";
            Approvals.Verify(TextFormatter.MergeBlocks(left, 25, right));
        }

        [Test]
        public void BlocksWhereTheLeftSideIsLongerAreMergedProperly()
        {
            var left =
                @"First line
Second line";
            var right = @"A";
            Approvals.Verify(TextFormatter.MergeBlocks(left, 25, right));
        }

        [Test]
        public void BlocksWhereTheRightSideIsLongerAreMergedProperly()
        {
            var left =
                @"First line
Second line";
            var right =
                @"A
B
C
D
E";
            Approvals.Verify(TextFormatter.MergeBlocks(left, 25, right));
        }
    }
}
