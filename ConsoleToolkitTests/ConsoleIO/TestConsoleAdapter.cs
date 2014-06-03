using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
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
    }
}