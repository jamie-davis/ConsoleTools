using System.Linq;
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
    public class TestConsoleAdapter
    {
        private ConsoleInterfaceForTesting _consoleInterface;
        private ConsoleAdapter _adapter;

        [SetUp]
        public void SetUp()
        {
            _consoleInterface = new ConsoleInterfaceForTesting();
            _adapter = new ConsoleAdapter(_consoleInterface);
        }

        [Test]
        public void LinesAreWrittenToTheConsole()
        {
            _adapter.WriteLine("Console {0}", "output");
            _adapter.WriteLine("More output.");

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void ConsoleColourChangesArePassedToConsoleInterface()
        {
            _adapter.WriteLine("Console {0}", "output".Red().BGYellow());
            _adapter.WriteLine("More output.".Green());
            _adapter.WriteLine("Back to plain.");

            Approvals.Verify(_consoleInterface.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void FullWidthTabularDataIsDisplayed()
        {
            _consoleInterface.WindowWidth = _adapter.BufferWidth;
            _adapter.WriteLine(RulerFormatter.MakeRuler(_adapter.BufferWidth));
            var data = Enumerable.Range(1, 10)
                .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });
            _adapter.FormatTable(data);

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void ShortWidthTabularDataIsDisplayed()
        {
            _adapter.WriteLine(RulerFormatter.MakeRuler(_adapter.BufferWidth));
            var data = Enumerable.Range(1, 10)
                .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });
            _adapter.FormatTable(data);

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void WrapLinesAreWordWrappedToConsole()
        {
            _adapter.WrapLine("Long console {0} that should require word wrapping to fit the display width, but remain short of the line end to illustrate WrapLine.", "output".Red().BGYellow());
            _adapter.WrapLine("More output, and hopefully more wrapping. Must ensure that there is enough text. Also WrapLine'd.".Green());
            _adapter.WrapLine("No wrapping needed.");

            Approvals.Verify(_consoleInterface.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void PiecemealWritesAreWordWrappedToConsole()
        {
            _adapter.Wrap("Long console {0} ", "output".Red().BGYellow());
            _adapter.Wrap("that should require word wrapping to");
            _adapter.Wrap(" fit the display width.");
            _adapter.Wrap(" This was ");
            _adapter.Wrap("split");
            _adapter.Wrap(" into");
            _adapter.Wrap(" many small writes");

            Approvals.Verify(_consoleInterface.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void RecordedOperationsCanBeDisplayedOnTheConsole()
        {
            var recorder = MakeRecording();

            _adapter.Write(recorder);
            _adapter.Write("XXX");
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void RecordedOperationsStartOnANewLine()
        {
            var recorder = MakeRecording();

            _adapter.Write("XXX");
            _adapter.Write(recorder);
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void RecordedOperationsCanBeDisplayedOnTheConsoleUsingWriteLine()
        {
            var recorder = MakeRecording();

            _adapter.WriteLine(recorder);
            _adapter.WriteLine("XXX");
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void RecordedOperationsDisplayedWithWriteLineStartOnANewLine()
        {
            var recorder = MakeRecording();

            _adapter.Write("XXX");
            _adapter.WriteLine(recorder);
            _adapter.WriteLine("XXX");
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        private static RecordingConsoleAdapter MakeRecording()
        {
            var recorder = new RecordingConsoleAdapter();
            recorder.Write("Write.");
            recorder.WriteLine("WriteLine.");
            recorder.Wrap("Some wrapped text ");
            recorder.Wrap("added in chunks, followed by ");
            recorder.WrapLine("a final wrap line.");
            var data = Enumerable.Range(1, 10)
                .Select(i => new {Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i))});
            recorder.FormatTable(data);
            recorder.Write("<--- END");
            return recorder;
        }
    }
}