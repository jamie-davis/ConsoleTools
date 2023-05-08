using System;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestRedirectedConsole
    {
        private RedirectedConsole _consoleOut;
        private StringWriter _buffer;
        private TextWriter _oldOut;

        [SetUp]
        public void SetUp()
        {
            _oldOut = Console.Out;
            _buffer = new StringWriter();
            Console.SetOut(_buffer);

            _consoleOut = new RedirectedConsole(ConsoleStream.Out, 80);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_oldOut);
        }


        [Test]
        public void TheCursorPositionAdvancesWhenTextIsOutput()
        {
            _consoleOut.Write("text");
            Assert.That(_consoleOut.CursorLeft, Is.EqualTo(4));
        }

        [Test]
        public void TextWiderThanTheBufferFlowsToNextLine()
        {
            var longLine = new string('X', _consoleOut.BufferWidth + 10);
            _consoleOut.Write(longLine);
            Approvals.Verify(RulerFormatter.MakeRuler(_consoleOut.BufferWidth) + Environment.NewLine + _buffer);
        }

        [Test]
        public void TextAsWideAsTheBufferFlowsCreatesNewLine()
        {
            var longLine = new string('X', _consoleOut.BufferWidth);
            _consoleOut.Write(longLine);
            Approvals.Verify(RulerFormatter.MakeRuler(_consoleOut.BufferWidth) + Environment.NewLine + _buffer);
        }

        [Test]
        public void TextAsWideAsTheBufferMovesCursorDownOneLine()
        {
            var longLine = new string('X', _consoleOut.BufferWidth);
            _consoleOut.Write(longLine);
            Assert.That(_consoleOut.CursorTop, Is.EqualTo(1));
        }

        [Test]
        public void TextAsWideAsTheBufferMovesCursorToStartOfNewLine()
        {
            var longLine = new string('X', _consoleOut.BufferWidth);
            _consoleOut.Write(longLine);
            Assert.That(_consoleOut.CursorLeft, Is.EqualTo(0));
        }
    }
}