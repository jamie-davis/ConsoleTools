using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestReportColumnAligner
    {
        [Fact]
        public void SingleLineIsFormatted()
        {
            var output = ReportColumnAligner.AlignColumns(new[] {10, 3, 5},
                new[] {new[] {"ten widexx"}, new[] {"3wd"}, new[] {"fivew"}});
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Fact]
        public void ExcessiveDataIsTrimmed()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 8, 2, 4 }, 
                new[] { new[] { "ten widexx" }, new[] { "3wd" }, new[] { "fivew" } });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Fact]
        public void ShortDataIsPadded()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 14, 6, 7 }, 
                new[] { new[] { "ten widexx" }, new[] { "3wd" }, new[] { "fivew" } });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Fact]
        public void MultiRowDataIsFormattedInColumns()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 14, 6, 7 }, 
                new[] { new[] { "ten widexx", "more", "too wide too wide" }, new[] { "3wd", "3mo", "1" }, new[] { "fivew", "again", "again" } });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Fact]
        public void InconsistentRowsAreTopAligned()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 10, 3, 5 }, 
                new[] { new[] { "ten widexx", "more" }, new[] { "3wd"}, new[] { "fivew", "again", "again", "again" } });
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Fact]
        public void InconsistentRowsAreBottomAligned()
        {
            var output = ReportColumnAligner.AlignColumns(new[] { 10, 3, 5 }, 
                new[] { new[] { "ten widexx", "more" }, new[] { "3wd"}, new[] { "fivew", "again", "again", "again" } },
                ColVerticalAligment.Bottom);
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Fact]
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