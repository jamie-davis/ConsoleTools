using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit;
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

        #region Types for test

        private class StringIntType
        {
            public StringIntType(string strValue, int intValue)
            {
                Str = strValue;
                Int = intValue;
            }

            public string Str { get; set; }
            public int Int { get; set; }

            public override string ToString()
            {
                return string.Format("[{0}, {1}]", Str, Int);
            }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            Toolkit.GlobalReset();
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

        [Test]
        public void AnAnonymousTypeIsFilledFromStdIn()
        {
            const string data = @"String line
150";
            var template = new {Str = "String line", Int = 150};
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput(template);
                Assert.That(item, Is.EqualTo(template));
            }
        }

        [Test]
        public void ATupleIsFilledFromStdInByTemplate()
        {
            const string data = @"String line
250";
            var template = new Tuple<string, int>("String line", 250);
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput(template);
                Assert.That(item, Is.EqualTo(template));
            }
        }

        [Test]
        public void ACustomTypeIsFilledFromStdInByTemplate()
        {
            const string data = @"String line
350";
            var template = new StringIntType("String line", 350);
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput(template);
                Assert.That(item.ToString(), Is.EqualTo(template.ToString()));
            }
        }

        [Test]
        public void ATupleIsFilledFromStdIn()
        {
            const string data = @"String line
250";
            var template = new Tuple<string, int>("String line", 250);
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput<Tuple<string, int>>();
                Assert.That(item, Is.EqualTo(template));
            }
        }

        [Test]
        public void ACustomTypeIsFilledFromStdIn()
        {
            const string data = @"String line
350";
            var template = new StringIntType("String line", 350);
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput<StringIntType>();
                Assert.That(item.ToString(), Is.EqualTo(template.ToString()));
            }
        }

        [Test]
        public void DefaultConfirmAcceptsY()
        {
            const string data = @"Y";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                Assert.That(_adapter.Confirm("Accepts Y"), Is.True);
            }
        }

        [Test]
        public void ConfirmDisplaysPrompt()
        {
            const string data = @"Y";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                _adapter.Confirm("Accepts Y?");
                Approvals.Verify(_consoleInterface.GetBuffer());
            }
        }

        [Test]
        public void DefaultConfirmAcceptsN()
        {
            const string data = @"N";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                Assert.That(_adapter.Confirm("Accepts N"), Is.False);
            }
        }

        [Test]
        public void ConfirmationYesOptionCanBeOverridden()
        {
            Toolkit.Options.OverrideConfirmOptions("T", "True", "F", "False");
            const string data = @"T";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                Assert.That(_adapter.Confirm("Accepts T"), Is.True);
            }
        }

        [Test]
        public void ConfirmationNoOptionCanBeOverridden()
        {
            Toolkit.Options.OverrideConfirmOptions("T", "True", "F", "False");
            const string data = @"F";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                Assert.That(_adapter.Confirm("Accepts F"), Is.False);
            }
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