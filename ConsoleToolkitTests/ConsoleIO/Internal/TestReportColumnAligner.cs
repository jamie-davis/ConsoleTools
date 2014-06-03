using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestReportColumnAligner
    {
        [Test]
        public void SingleLineIsFormatted()
        {
            var output = ReportColumnAligner.AlignColumns(new[] {10, 3, 5},
                new[] {new[] {"ten widexx"}, new[] {"3wd"}, new[] {"fivew"}});
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Test]
        public void ExcessiveDataIsTrimmed()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 8, 2, 4 }, 
                new[] { new[] { "ten widexx" }, new[] { "3wd" }, new[] { "fivew" } });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Test]
        public void ShortDataIsPadded()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 14, 6, 7 }, 
                new[] { new[] { "ten widexx" }, new[] { "3wd" }, new[] { "fivew" } });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Test]
        public void MultiRowDataIsFormattedInColumns()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 14, 6, 7 }, 
                new[] { new[] { "ten widexx", "more", "too wide too wide" }, new[] { "3wd", "3mo", "1" }, new[] { "fivew", "again", "again" } });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Test]
        public void InconsistentRowsAreTopAligned()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 10, 3, 5 }, 
                new[] { new[] { "ten widexx", "more" }, new[] { "3wd"}, new[] { "fivew", "again", "again", "again" } });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Test]
        public void InconsistentRowsAreBottomAligned()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 10, 3, 5 }, 
                new[] { new[] { "ten widexx", "more" }, new[] { "3wd"}, new[] { "fivew", "again", "again", "again" } },
                ColVerticalAligment.Bottom);
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Test]
        public void ColourDataIsNotCountedInTheWidth()
        {
            var output = ReportColumnAligner.AlignColumns(new[] {14, 6, 7},
                new[]
                {
                    new[] {"ten widexx", "more", "XXXXXXX".Red().BGBlack()},
                    new[] {"3wd", "3mo", "1"},
                    new[] {"fivew", "again", "again"}
                });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }
    }
}