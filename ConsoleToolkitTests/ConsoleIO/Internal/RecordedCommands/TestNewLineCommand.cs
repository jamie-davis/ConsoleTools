using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestNewLineCommand
    {
        private ReplayBuffer _buffer;
        private const int TestBufferWidth = 20;

        [SetUp]
        public void SetUp()
        {
            _buffer = new ReplayBuffer(TestBufferWidth);
        }

        [Test]
        public void WriteCommandReplaysWriteOperation()
        {
            var command = new NewLineCommand();
            
            _buffer.Write("XXX");
            command.Replay(_buffer);
            _buffer.Write("YYY");

            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Test]
        public void FirstWordLengthIsZero()
        {
            var command = new NewLineCommand();
            Assert.That(command.GetFirstWordLength(4), Is.EqualTo(0));            
        }

        [Test]
        public void LongestWordLengthIsZero()
        {
            var command = new NewLineCommand();
            Assert.That(command.GetLongestWordLength(4), Is.EqualTo(0));            
        }
    }
}
