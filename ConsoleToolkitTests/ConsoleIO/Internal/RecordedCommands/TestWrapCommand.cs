using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestWrapCommand
    {
        private ReplayBuffer _buffer;
        private const int TestBufferWidth = 20;

        [SetUp]
        public void SetUp()
        {
            _buffer = new ReplayBuffer(TestBufferWidth);
        }

        [Test]
        public void WrapCommandReplaysWrapOperation()
        {
            var command = new WrapCommand("XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXX");

            _buffer.Write("===");
            command.Replay(_buffer);
            _buffer.Write("YYY");

            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Test]
        public void LongestWordLengthIsReturned()
        {
            var command = new WrapCommand("Test text with one longer word.");
            Assert.That(command.GetLongestWordLength(4), Is.EqualTo("longer".Length));
        }

        [Test]
        public void FirstWordLengthIsReturned()
        {
            var command = new WrapCommand("Test text with one longer word.");
            Assert.That(command.GetFirstWordLength(4), Is.EqualTo("Test".Length));
        }

        [Test]
        public void LongestWordLengthIsZeroForEmptyString()
        {
            var command = new WrapCommand(string.Empty);
            Assert.That(command.GetLongestWordLength(4), Is.EqualTo(0));
        }

        [Test]
        public void FirstWordLengthIsZeroForEmptyString()
        {
            var command = new WrapCommand(string.Empty);
            Assert.That(command.GetFirstWordLength(4), Is.EqualTo(0));
        }

    }
}
