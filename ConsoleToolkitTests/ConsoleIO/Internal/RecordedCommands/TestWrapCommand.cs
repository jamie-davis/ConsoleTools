using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [UseReporter(typeof(CustomReporter))]
    public class TestWrapCommand
    {
        private ReplayBuffer _buffer;
        private const int TestBufferWidth = 20;
        public TestWrapCommand()
        {
            _buffer = new ReplayBuffer(TestBufferWidth);
        }

        [Fact]
        public void WrapCommandReplaysWrapOperation()
        {
            var command = new WrapCommand("XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXX");

            _buffer.Write("===");
            command.Replay(_buffer);
            _buffer.Write("YYY");

            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Fact]
        public void LongestWordLengthIsReturned()
        {
            var command = new WrapCommand("Test text with one longer word.");
            Assert.Equal("longer".Length, command.GetLongestWordLength(4));
        }

        [Fact]
        public void FirstWordLengthIsReturned()
        {
            var command = new WrapCommand("Test text with one longer word.");
            Assert.Equal("Test".Length, command.GetFirstWordLength(4));
        }

        [Fact]
        public void LongestWordLengthIsZeroForEmptyString()
        {
            var command = new WrapCommand(string.Empty);
            Assert.Equal(0, command.GetLongestWordLength(4));
        }

        [Fact]
        public void FirstWordLengthIsZeroForEmptyString()
        {
            var command = new WrapCommand(string.Empty);
            Assert.Equal(0, command.GetFirstWordLength(4));
        }

    }
}
