using System;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestRedirectedConsole
    {
        private RedirectedConsole _console;
        private StringWriter _buffer;
        private TextWriter _oldOut;

        [SetUp]
        public void SetUp()
        {
            _console = new RedirectedConsole();
            _buffer = new StringWriter();
            _oldOut = Console.Out;
            Console.SetOut(_buffer);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_oldOut);
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
            Approvals.Verify(RulerFormatter.MakeRuler(_console.BufferWidth) + Environment.NewLine + _buffer);
        }

        [Test]
        public void TextAsWideAsTheBufferFlowsCreatesNewLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Approvals.Verify(RulerFormatter.MakeRuler(_console.BufferWidth) + Environment.NewLine + _buffer);
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