using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestOldConsoleAdapter
    {
        private OldConsoleAdapter _adapter;
        private StringWriter _consoleOutput;
        private StringWriter _errorOutput;
        private int _width;

        [SetUp]
        public void SetUp()
        {
            var ruler = "12345678901234567890123456789012345678901234567890";
            _consoleOutput = new StringWriter();
            _consoleOutput.WriteLine(ruler);
            _errorOutput = new StringWriter();
            _errorOutput.WriteLine(ruler);
            _width = 50;
            _adapter = new OldConsoleAdapter(_consoleOutput, _errorOutput, _width);
        }

        [Test]
        public void TextIsWordWrappedOnConsole()
        {
            _adapter.WrapOutput("More than 50 character text which should be word wrapped at 50.");
            Approvals.Verify(_consoleOutput);
        }

        [Test]
        public void TextIsWordWrappedOnError()
        {
            _adapter.WrapError("More than 50 character text which should be word wrapped at 50.");
            Approvals.Verify(_errorOutput);
        }

        [Test]
        public void ObjectsArePrintedInColumns()
        {
            var items = new[] {"The quick", "Brown fox", "Jumped over", "The", "Lazy", "Dog", ""}
                .Select(t => new {Text = t, Len = t.Length});
            _adapter.DisplayColumns(items);
            Console.WriteLine(_consoleOutput);
            Approvals.Verify(_consoleOutput);
        }

        [Test]
        public void HeadingTextIsIncludedInColumnWidthWhenObjectsArePrintedInColumns()
        {
            var items = new[] {"The quick", "Brown fox", "Jumped over", "The", "Lazy", "Dog", ""}
                .Select(t => new {TextWithALongName = t, Len = t.Length});
            _adapter.DisplayColumns(items);
            Console.WriteLine(_consoleOutput);
            Approvals.Verify(_consoleOutput);
        }

        [Test]
        public void WhenSpecifiedObjectsArePrintedInColumnsWithoutHeadings()
        {
            var items = new[] {"The quick", "Brown fox", "Jumped over", "The", "Lazy", "Dog", ""}
                .Select(t => new {Text = t, Len = t.Length});
            _adapter.DisplayColumns(items, false);
            Console.WriteLine(_consoleOutput);
            Approvals.Verify(_consoleOutput);
        }

        [Test]
        public void ObjectsAreDisplayedInColumnsOnErrorStream()
        {
            var items = new[] {"The quick", "Brown fox", "Jumped over", "The", "Lazy", "Dog", ""}
                .Select(t => new {Text = t, Len = t.Length});
            _adapter.DisplayColumnsOnError(items);
            Approvals.Verify(_errorOutput);
        }
    }
}
