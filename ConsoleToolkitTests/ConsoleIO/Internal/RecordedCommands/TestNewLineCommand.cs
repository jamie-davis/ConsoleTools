using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [UseReporter(typeof (CustomReporter))]
    public class TestNewLineCommand
    {
        private ReplayBuffer _buffer;
        private const int TestBufferWidth = 20;
        public TestNewLineCommand()
        {
            _buffer = new ReplayBuffer(TestBufferWidth);
        }

        [Fact]
        public void WriteCommandReplaysWriteOperation()
        {
            var command = new NewLineCommand();
            
            _buffer.Write("XXX");
            command.Replay(_buffer);
            _buffer.Write("YYY");

            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Fact]
        public void FirstWordLengthIsZero()
        {
            var command = new NewLineCommand();
            Assert.Equal(0, command.GetFirstWordLength(4));            
        }

        [Fact]
        public void LongestWordLengthIsZero()
        {
            var command = new NewLineCommand();
            Assert.Equal(0, command.GetLongestWordLength(4));            
        }
    }
}
