using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [UseReporter(typeof (CustomReporter))]
    public class TestReplayBuffer
    {
        private const int TestBufferWidth = 20;

        private ReplayBuffer _buffer;
        public TestReplayBuffer()
        {
            _buffer = new ReplayBuffer(TestBufferWidth);
        }

        [Fact]
        public void TextIsOutputToBuffer()
        {
            _buffer.Write("text");
            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Fact]
        public void TextWrapsToNextLineIfBufferWidthExceeded()
        {
            _buffer.Write("text that exceeds the width of the buffer." );
            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void AWrappingLineBreakIsAddedIfWriteAddsNewLine()
        {
            _buffer.Write("text that exceeds the width of the buffer." );
            Console.WriteLine(GetBufferResult());
            Assert.Equal(2, _buffer.WordWrapLineBreakCount);
        }

        [Fact]
        public void NewLinesAreNotAutoStartedWithoutData()
        {
            _buffer.Write(new string('X', TestBufferWidth));
            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void IfTheCurrentLineIsFullANewLineStarts()
        {
            //fill the current line
            _buffer.Write(new string('X', TestBufferWidth));

            //start the next line
            _buffer.Write("Y");

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void IfTheCurrentLineIsFullALineBreakWrapIsAdded()
        {
            //fill the current line
            _buffer.Write(new string('X', TestBufferWidth));

            //start the next line
            _buffer.Write("Y");

            Console.WriteLine(GetBufferResult());
            Assert.Equal(1, _buffer.WordWrapLineBreakCount);
        }

        [Fact]
        public void ExplicitNewLineStartsANewLine()
        {
            _buffer.Write("Short line");
            _buffer.NewLine();

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void ExplicitNewLineDoesNotAddToWrapCount()
        {
            _buffer.Write("Short line");
            _buffer.NewLine();

            Console.WriteLine(GetBufferResult());
            Assert.Equal(0, _buffer.WordWrapLineBreakCount);
        }

        [Fact]
        public void NewLineWhenTheCurrentLineIsFullJustStartsANewLine()
        {
            //fill the current line
            _buffer.Write(new string('X', TestBufferWidth));

            _buffer.NewLine();
            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void NewLineWhenTheCurrentLineIsFullDoesNotAddAToWrapCount()
        {
            //fill the current line
            _buffer.Write(new string('X', TestBufferWidth));

            _buffer.NewLine();
            Console.WriteLine(GetBufferResult());
            Assert.Equal(0, _buffer.WordWrapLineBreakCount);
        }

        [Fact]
        public void DataContainingEmbeddedNewLineIsDisplayedCorrectly()
        {
            _buffer.Write("New line->\r\n" + new string('X', TestBufferWidth));

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void DataContainingEmbeddedNewLineDoesNotIncrementWrapCount()
        {
            _buffer.Write("New line->\r\n" + new string('X', TestBufferWidth));

            Console.WriteLine(GetBufferResult());
            Assert.Equal(0, _buffer.WordWrapLineBreakCount);
        }

        [Fact]
        public void ColourInstructionsAreIncorporated()
        {
            var text = "Red".Red()
                + " and " + "yellow".Yellow() 
                + " and " + "pink".Magenta() 
                + " and " + "green".Green();
            _buffer.Write(text);

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void PreSplitColourItemsCanBeWritten()
        {
            var text = "Red".Red()
                + " and " + "yellow".Yellow() 
                + " and " + "pink".Magenta() 
                + " and " + "green".Green();
            _buffer.Write(ColourControlSplitter.Split(text));

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void SingleColourSplitItemsCanBeWritten()
        {
            var text = "Red".Red()
                + " and " + "yellow".Yellow() 
                + " and " + "pink".Magenta() 
                + " and " + "green".Green();
            foreach (var colourControlItem in ColourControlSplitter.Split(text))
                _buffer.Write(colourControlItem);

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void WrapWordWraps()
        {
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void WrapWordWrapAddsToWrapCount()
        {
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);

            Console.WriteLine(GetBufferResult());
            Assert.Equal(2, _buffer.WordWrapLineBreakCount);
        }

        [Fact]
        public void WrapWordWrapsFromHangingIndent()
        {
            _buffer.Write("XXXX XXX ");
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void WrapCountsWordWrapsFromHangingIndent()
        {
            _buffer.Write("XXXX XXX ");
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);

            Console.WriteLine(GetBufferResult());
            Assert.Equal(3, _buffer.WordWrapLineBreakCount);
        }

        [Fact]
        public void WrappedTextDoesNotPerformWriteLineAtEnd()
        {
            _buffer.Write("XXXX XXX ");
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);
            _buffer.Write(" YYY");

            Approvals.Verify(GetBufferResult());
        }

        [Fact]
        public void CursorLeftTracksCursorPosition()
        {
            _buffer.Write("XXXX");
            Assert.Equal(4, _buffer.CursorLeft);
        }

        [Fact]
        public void CursorLeftIsZeroAfterNewLine()
        {
            _buffer.Write("XXXX");
            _buffer.NewLine();
            Assert.Equal(0, _buffer.CursorLeft);
        }

        private string GetBufferResult()
        {
            var ruler = RulerFormatter.MakeRuler(TestBufferWidth);
            var bufferLines = _buffer.ToLines();

            var result = new[] {ruler}.Concat(bufferLines);

            var bufferResult = result.JoinWith(Environment.NewLine);
            return bufferResult;
        }
    }
}
