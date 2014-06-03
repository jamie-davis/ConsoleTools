using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestConsoleInterfaceForTesting
    {
        private ConsoleInterfaceForTesting _console;

        [SetUp]
        public void SetUp()
        {
            _console = new ConsoleInterfaceForTesting();
        }

        [Test]
        public void TheCursorPositionAdvancesWhenTextIsOutput()
        {
            _console.Write("text");
            Assert.That(_console.CursorLeft, Is.EqualTo(4));
        }

        [Test]
        public void TextWiderThanTheBufferFlowsToNextLine()
        {
            var longLine = new string('X', _console.BufferWidth + 10);
            _console.Write(longLine);
            Approvals.Verify(_console.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void TextAsWideAsTheBufferFlowsCreatesNewLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Approvals.Verify(_console.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void TextAsWideAsTheBufferMovesCursorDownOneLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Assert.That(_console.CursorTop, Is.EqualTo(1));
        }

        [Test]
        public void TextAsWideAsTheBufferMovesCursorToStartOfNewLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Assert.That(_console.CursorLeft, Is.EqualTo(0));
        }
    }
}
