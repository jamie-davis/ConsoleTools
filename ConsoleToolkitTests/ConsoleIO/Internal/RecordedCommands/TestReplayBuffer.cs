using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [TestFixture]
    [UseReporter(typeof (DiffReporter))]
    public class TestReplayBuffer
    {
        private const int TestBufferWidth = 20;

        private ReplayBuffer _buffer;

        [SetUp]
        public void SetUp()
        {
            _buffer = new ReplayBuffer(TestBufferWidth);
        }

        [Test]
        public void TextIsOutputToBuffer()
        {
            _buffer.Write("text");
            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Test]
        public void TextWrapsToNextLineIfBufferWidthExceeded()
        {
            _buffer.Write("text that exceeds the width of the buffer." );
            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void AWrappingLineBreakIsAddedIfWriteAddsNewLine()
        {
            _buffer.Write("text that exceeds the width of the buffer." );
            Console.WriteLine(GetBufferResult());
            Assert.That(_buffer.WordWrapLineBreakCount, Is.EqualTo(2));
        }

        [Test]
        public void NewLinesAreNotAutoStartedWithoutData()
        {
            _buffer.Write(new string('X', TestBufferWidth));
            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void IfTheCurrentLineIsFullANewLineStarts()
        {
            //fill the current line
            _buffer.Write(new string('X', TestBufferWidth));

            //start the next line
            _buffer.Write("Y");

            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void IfTheCurrentLineIsFullALineBreakWrapIsAdded()
        {
            //fill the current line
            _buffer.Write(new string('X', TestBufferWidth));

            //start the next line
            _buffer.Write("Y");

            Console.WriteLine(GetBufferResult());
            Assert.That(_buffer.WordWrapLineBreakCount, Is.EqualTo(1));
        }

        [Test]
        public void ExplicitNewLineStartsANewLine()
        {
            _buffer.Write("Short line");
            _buffer.NewLine();

            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void ExplicitNewLineDoesNotAddToWrapCount()
        {
            _buffer.Write("Short line");
            _buffer.NewLine();

            Console.WriteLine(GetBufferResult());
            Assert.That(_buffer.WordWrapLineBreakCount, Is.EqualTo(0));
        }

        [Test]
        public void NewLineWhenTheCurrentLineIsFullJustStartsANewLine()
        {
            //fill the current line
            _buffer.Write(new string('X', TestBufferWidth));

            _buffer.NewLine();
            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void NewLineWhenTheCurrentLineIsFullDoesNotAddAToWrapCount()
        {
            //fill the current line
            _buffer.Write(new string('X', TestBufferWidth));

            _buffer.NewLine();
            Console.WriteLine(GetBufferResult());
            Assert.That(_buffer.WordWrapLineBreakCount, Is.EqualTo(0));
        }

        [Test]
        public void DataContainingEmbeddedNewLineIsDisplayedCorrectly()
        {
            _buffer.Write("New line->\r\n" + new string('X', TestBufferWidth));

            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void DataContainingEmbeddedNewLineDoesNotIncrementWrapCount()
        {
            _buffer.Write("New line->\r\n" + new string('X', TestBufferWidth));

            Console.WriteLine(GetBufferResult());
            Assert.That(_buffer.WordWrapLineBreakCount, Is.EqualTo(0));
        }

        [Test]
        public void ColourInstructionsAreIncorporated()
        {
            var text = "Red".Red()
                + " and " + "yellow".Yellow() 
                + " and " + "pink".Magenta() 
                + " and " + "green".Green();
            _buffer.Write(text);

            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void PreSplitColourItemsCanBeWritten()
        {
            var text = "Red".Red()
                + " and " + "yellow".Yellow() 
                + " and " + "pink".Magenta() 
                + " and " + "green".Green();
            _buffer.Write(ColourControlSplitter.Split(text));

            Approvals.Verify(GetBufferResult());
        }

        [Test]
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

        [Test]
        public void WrapWordWraps()
        {
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);

            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void WrapWordWrapAddsToWrapCount()
        {
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);

            Console.WriteLine(GetBufferResult());
            Assert.That(_buffer.WordWrapLineBreakCount, Is.EqualTo(2));
        }

        [Test]
        public void WrapWordWrapsFromHangingIndent()
        {
            _buffer.Write("XXXX XXX ");
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);

            Approvals.Verify(GetBufferResult());
        }

        [Test]
        public void WrapCountsWordWrapsFromHangingIndent()
        {
            _buffer.Write("XXXX XXX ");
            var text = "Red".Red()
                + " and green".Green()
                + " text that wraps around at least one line.";
            _buffer.Wrap(text);

            Console.WriteLine(GetBufferResult());
            Assert.That(_buffer.WordWrapLineBreakCount, Is.EqualTo(3));
        }

        [Test]
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

        [Test]
        public void CursorLeftTracksCursorPosition()
        {
            _buffer.Write("XXXX");
            Assert.That(_buffer.CursorLeft, Is.EqualTo(4));
        }

        [Test]
        public void CursorLeftIsZeroAfterNewLine()
        {
            _buffer.Write("XXXX");
            _buffer.NewLine();
            Assert.That(_buffer.CursorLeft, Is.EqualTo(0));
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
