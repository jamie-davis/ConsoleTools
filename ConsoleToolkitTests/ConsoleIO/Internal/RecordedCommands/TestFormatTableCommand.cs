using System;
using System.Linq;
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
    public class TestFormatTableCommand
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
            var data = Enumerable.Range(1, 10)
                     .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });

            var command = FormatTableCommandFactory.Make(data);
            
            _buffer.Write("XXX");
            _buffer.NewLine();
            command.Replay(_buffer);
            _buffer.Write("YYY");

            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Test]
        public void WriteCommandReplaysWriteOperationWithOptions()
        {
            var data = Enumerable.Range(1, 10)
                     .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });

            var command = FormatTableCommandFactory.Make(data, options: ReportFormattingOptions.Default | ReportFormattingOptions.OmitHeadings);
            
            _buffer.Write("XXX");
            _buffer.NewLine();
            command.Replay(_buffer);
            _buffer.Write("YYY");

            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }

        [Test]
        public void WriteCommandReplaysReportOperation()
        {
            var data = Enumerable.Range(1, 10)
                     .AsReport(rep => rep.AddColumn(i => i, col => col.Heading("Number")));

            var command = FormatTableCommandFactory.Make(data);
            
            _buffer.Write("XXX");
            _buffer.NewLine();
            command.Replay(_buffer);
            _buffer.Write("YYY");

            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }


        [Test]
        public void FirstWordLengthIsCalculated()
        {
            var data = Enumerable.Range(1, 10)
                     .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah blu", i)) });

            var command = FormatTableCommandFactory.Make(data);
            Assert.That(command.GetFirstWordLength(4), Is.EqualTo("Number String".Length));
        }

        [Test]
        public void LongestWordLengthIsCalculated()
        {
            var data = Enumerable.Range(1, 10)
                     .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });

            var command = FormatTableCommandFactory.Make(data);
            Assert.That(command.GetLongestWordLength(4), Is.EqualTo("Number String".Length));
        }

    }
}