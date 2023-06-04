using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [UseReporter(typeof (CustomReporter))]
    public class TestWriteCommand
    {
        private ReplayBuffer _buffer;
        private const int TestBufferWidth = 20;
        public TestWriteCommand()
        {
            _buffer = new ReplayBuffer(TestBufferWidth);
        }

        [Fact]
        public void WriteCommandReplaysWriteOperation()
        {
            var command = new WriteCommand("text");
            command.Replay(_buffer);
            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Fact]
        public void LongestWordLengthIsReturned()
        {
            var command = new WriteCommand("Test text with one longer word.");
            Assert.Equal("longer".Length, command.GetLongestWordLength(4));
        }

        [Fact]
        public void FirstWordLengthIsReturned()
        {
            var command = new WriteCommand("Test text with one longer word.");
            Assert.Equal("Test".Length, command.GetFirstWordLength(4));
        }

        [Fact]
        public void LongestWordLengthIsZeroForEmptyString()
        {
            var command = new WriteCommand(string.Empty);
            Assert.Equal(0, command.GetLongestWordLength(4));
        }

        [Fact]
        public void FirstWordLengthIsZeroForEmptyString()
        {
            var command = new WriteCommand(string.Empty);
            Assert.Equal(0, command.GetFirstWordLength(4));
        }
    }
}