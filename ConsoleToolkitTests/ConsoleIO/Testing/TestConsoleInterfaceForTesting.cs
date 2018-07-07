using System;
using System.Collections.Generic;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Testing
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

        [Test]
        public void ReadLineThrowsWhenNoInputIsSet()
        {
            Assert.That(() => _console.ReadLine(), Throws.InstanceOf(typeof(Exception)));
        }

        [Test]
        public void ReadLineCanReadInputFromAStream()
        {
            var data = @"First line.
Second line.
Third line.";
            using (var stream = new StringReader(data))
            {
                var lines = new List<string>();
                _console.SetInputStream(stream);
                string line;
                while ((line = _console.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                Assert.That(lines.JoinWith(Environment.NewLine), Is.EqualTo(data));
            }
        }

        [Test]
        public void InterfaceIsInValidStateAfterWidthChange()
        {
            _console.BufferWidth = 132;
            _console.WindowWidth = 132;
            _console.Write("text text text");
            Assert.That(_console.CursorLeft, Is.EqualTo(14));
        }
    }
}
