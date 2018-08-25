using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestWriteCommand
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
            var command = new WriteCommand("text");
            command.Replay(_buffer);
            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Test]
        public void LongestWordLengthIsReturned()
        {
            var command = new WriteCommand("Test text with one longer word.");
            Assert.That(command.GetLongestWordLength(4), Is.EqualTo("longer".Length));
        }

        [Test]
        public void FirstWordLengthIsReturned()
        {
            var command = new WriteCommand("Test text with one longer word.");
            Assert.That(command.GetFirstWordLength(4), Is.EqualTo("Test".Length));
        }

        [Test]
        public void LongestWordLengthIsZeroForEmptyString()
        {
            var command = new WriteCommand(string.Empty);
            Assert.That(command.GetLongestWordLength(4), Is.EqualTo(0));
        }

        [Test]
        public void FirstWordLengthIsZeroForEmptyString()
        {
            var command = new WriteCommand(string.Empty);
            Assert.That(command.GetFirstWordLength(4), Is.EqualTo(0));
        }
    }
}