using System;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
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
        public TestConsoleAdapter()
        {
            Toolkit.GlobalReset();
            _consoleInterface = new ConsoleInterfaceForTesting();
            _adapter = new ConsoleAdapter(_consoleInterface);
        }

        [Fact]
        public void LinesAreWrittenToTheConsole()
        {
            _adapter.WriteLine("Console {0}", "output");
            _adapter.WriteLine("More output.");

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void AnAnonymousTypeIsFilledFromStdIn()
        {
            const string data = @"String line
150";
            var template = new {Str = "String line", Int = 150};
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput(template);
                Assert.Equal(template, item);
            }
        }

        [Fact]
        public void ATupleIsFilledFromStdInByTemplate()
        {
            const string data = @"String line
250";
            var template = new Tuple<string, int>("String line", 250);
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput(template);
                Assert.Equal(template, item);
            }
        }

        [Fact]
        public void ACustomTypeIsFilledFromStdInByTemplate()
        {
            const string data = @"String line
350";
            var template = new StringIntType("String line", 350);
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput(template);
                Assert.Equal(template.ToString(), item.ToString());
            }
        }

        [Fact]
        public void ATupleIsFilledFromStdIn()
        {
            const string data = @"String line
250";
            var template = new Tuple<string, int>("String line", 250);
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput<Tuple<string, int>>();
                Assert.Equal(template, item);
            }
        }

        [Fact]
        public void ACustomTypeIsFilledFromStdIn()
        {
            const string data = @"String line
350";
            var template = new StringIntType("String line", 350);
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                var item = _adapter.ReadInput<StringIntType>();
                Assert.Equal(template.ToString(), item.ToString());
            }
        }

        [Fact]
        public void DefaultConfirmAcceptsY()
        {
            const string data = @"Y";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                _adapter.Confirm("Accepts Y").Should().BeTrue();
            }
        }

        [Fact]
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

        [Fact]
        public void DefaultConfirmAcceptsN()
        {
            const string data = @"N";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                _adapter.Confirm("Accepts N").Should().BeFalse();
            }
        }

        [Fact]
        public void ConfirmationYesOptionCanBeOverridden()
        {
            Toolkit.Options.OverrideConfirmOptions("T", "True", "F", "False");
            const string data = @"T";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                _adapter.Confirm("Accepts T").Should().BeTrue();
            }
        }

        [Fact]
        public void ConfirmationNoOptionCanBeOverridden()
        {
            Toolkit.Options.OverrideConfirmOptions("T", "True", "F", "False");
            const string data = @"F";
            using (var s = new StringReader(data))
            {
                _consoleInterface.SetInputStream(s);
                _adapter.Confirm("Accepts F").Should().BeFalse();
            }
        }
    }
}