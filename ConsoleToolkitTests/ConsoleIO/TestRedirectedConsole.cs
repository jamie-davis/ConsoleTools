using System;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO
{
    [UseReporter(typeof (CustomReporter))]
    public class TestRedirectedConsole : IDisposable
    {
        private RedirectedConsole _consoleOut;
        private StringWriter _buffer;
        private TextWriter _oldOut;
        public TestRedirectedConsole()
        {
            _oldOut = Console.Out;
            _buffer = new StringWriter();
            Console.SetOut(_buffer);

            _consoleOut = new RedirectedConsole(ConsoleStream.Out, 80);
        }

        void IDisposable.Dispose()
        {
            Console.SetOut(_oldOut);
        }


        [Fact]
        public void TheCursorPositionAdvancesWhenTextIsOutput()
        {
            _consoleOut.Write("text");
            Assert.Equal(4, _consoleOut.CursorLeft);
        }

        [Fact]
        public void TextWiderThanTheBufferFlowsToNextLine()
        {
            var longLine = new string('X', _consoleOut.BufferWidth + 10);
            _consoleOut.Write(longLine);
            Approvals.Verify(RulerFormatter.MakeRuler(_consoleOut.BufferWidth) + Environment.NewLine + _buffer);
        }

        [Fact]
        public void TextAsWideAsTheBufferFlowsCreatesNewLine()
        {
            var longLine = new string('X', _consoleOut.BufferWidth);
            _consoleOut.Write(longLine);
            Approvals.Verify(RulerFormatter.MakeRuler(_consoleOut.BufferWidth) + Environment.NewLine + _buffer);
        }

        [Fact]
        public void TextAsWideAsTheBufferMovesCursorDownOneLine()
        {
            var longLine = new string('X', _consoleOut.BufferWidth);
            _consoleOut.Write(longLine);
            Assert.Equal(1, _consoleOut.CursorTop);
        }

        [Fact]
        public void TextAsWideAsTheBufferMovesCursorToStartOfNewLine()
        {
            var longLine = new string('X', _consoleOut.BufferWidth);
            _consoleOut.Write(longLine);
            Assert.Equal(0, _consoleOut.CursorLeft);
        }
    }
}