using System;
using System.Collections.Generic;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Testing
{
    [UseReporter(typeof (CustomReporter))]
    public class TestConsoleInterfaceForTesting
    {
        private ConsoleInterfaceForTesting _console;
        public TestConsoleInterfaceForTesting()
        {
            _console = new ConsoleInterfaceForTesting();
        }

        [Fact]
        public void TheCursorPositionAdvancesWhenTextIsOutput()
        {
            _console.Write("text");
            Assert.Equal(4, _console.CursorLeft);
        }

        [Fact]
        public void TextWiderThanTheBufferFlowsToNextLine()
        {
            var longLine = new string('X', _console.BufferWidth + 10);
            _console.Write(longLine);
            Approvals.Verify(_console.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Fact]
        public void TextAsWideAsTheBufferFlowsCreatesNewLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Approvals.Verify(_console.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Fact]
        public void TextAsWideAsTheBufferMovesCursorDownOneLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Assert.Equal(1, _console.CursorTop);
        }

        [Fact]
        public void TextAsWideAsTheBufferMovesCursorToStartOfNewLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Assert.Equal(0, _console.CursorLeft);
        }

        [Fact]
        public void ReadLineThrowsWhenNoInputIsSet()
        {
            Action act = () => _console.ReadLine();
            act.Should().Throw<Exception>();
        }

        [Fact]
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
                Assert.Equal(data, lines.JoinWith(Environment.NewLine));
            }
        }

        [Fact]
        public void InterfaceIsInValidStateAfterWidthChange()
        {
            _console.BufferWidth = 132;
            _console.WindowWidth = 132;
            _console.Write("text text text");
            Assert.Equal(14, _console.CursorLeft);
        }
    }
}
