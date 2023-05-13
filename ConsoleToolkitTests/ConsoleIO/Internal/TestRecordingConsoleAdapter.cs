using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestRecordingConsoleAdapter
    {
        private RecordingConsoleAdapter _adapter;
        public TestRecordingConsoleAdapter()
        {
            _adapter = new RecordingConsoleAdapter();
        }

        [Fact]
        public void AdapterPlaysBackWrite()
        {
            _adapter.Write("text");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void AdapterPlaysBackWriteWithArgs()
        {
            _adapter.Write("text {0}", "with insert");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void AdapterPlaysBackWriteLineWithArgs()
        {
            _adapter.WriteLine("text {0}", "with insert");
            _adapter.WriteLine("next line");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void AdapterPlaysBackWriteLine()
        {
            _adapter.WriteLine("text");
            _adapter.WriteLine();
            _adapter.Write("plain write");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void AdapterPlaysBackTabularDataFormatting()
        {
            var data = Enumerable.Range(1, 10)
                                 .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });

            _adapter.FormatTable(data);
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void AdapterPlaysBackWrap()
        {
            _adapter.Wrap("Demonstrate that wrapping \"wrap {0} up\" performs wrapping.", "it");
            _adapter.Wrap(" Also, wrapped data can be written in chunks.");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void AdapterPlaysBackWrapLine()
        {
            _adapter.WrapLine("Demonstrate that wrapping \"wrap {0} up\" performs wrapping.", "it");
            _adapter.WrapLine("Also, wrapped data can be written in lines.");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void AdapterPlaysBackAllCommands()
        {
            var data = Enumerable.Range(1, 10)
                                 .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });

            _adapter.Write("This is the output from a single render operation. It contains a fixed line (this one) ");
            _adapter.Write("split across two Write operations (this is from the second one). Next it will contain ");
            _adapter.WriteLine("a WriteLine (this one) which will be followed by a table.");
            _adapter.WriteLine();
            _adapter.FormatTable(data);
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void RecordedOperationsCanBeDisplayedInARecording()
        {
            var recorder = MakeRecording();

            _adapter.Write(recorder);
            _adapter.Write("XXX");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void RecordedOperationsStartOnANewLine()
        {
            var recorder = MakeRecording();

            _adapter.Write("XXX");
            _adapter.Write(recorder);
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void RecordedOperationsCanBeDisplayedOnTheConsoleUsingWriteLine()
        {
            var recorder = MakeRecording();

            _adapter.WriteLine(recorder);
            _adapter.WriteLine("XXX");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void RecordedOperationsDisplayedWithWriteLineStartOnANewLine()
        {
            var recorder = MakeRecording();

            _adapter.Write("XXX");
            _adapter.WriteLine(recorder);
            _adapter.WriteLine("XXX");
            int wrappedLines;
            var output = _adapter.Render(40, out wrappedLines);
            Approvals.Verify(output.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void GetFirstWordLengthReturnsLengthFromFirstStep()
        {
            var recorder = MakeRecording();
            Assert.Equal("Write.".Length, recorder.GetFirstWordLength(4));
        }

        [Fact]
        public void GetLongestWordLengthReturnsLongestWordInAllSteps()
        {
            var recorder = MakeRecording();
            Assert.Equal("Number String".Length, recorder.GetLongestWordLength(4));
        }

        [Fact]
        public void WrappingLineBreaksAreCalculated()
        {
            var recorder = MakeRecording();
            int wrappedLines;
            Console.WriteLine(recorder.Render(20, out wrappedLines).JoinWith(Environment.NewLine));
            Assert.Equal(23, recorder.CountWordWrapLineBreaks(20));
        }

        private static RecordingConsoleAdapter MakeRecording()
        {
            var recorder = new RecordingConsoleAdapter();
            recorder.Write("Write.");
            recorder.WriteLine("WriteLine.");
            recorder.Wrap("Some wrapped text ");
            recorder.Wrap("added in chunks, followed by ");
            recorder.WrapLine("a final wrap line.");
            var data = Enumerable.Range(1, 10)
                .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });
            recorder.FormatTable(data);
            recorder.Write("<--- END");
            return recorder;
        }

    }
}
