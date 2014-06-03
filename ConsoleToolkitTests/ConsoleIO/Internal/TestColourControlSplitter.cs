using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestColourControlSplitter
    {
        [Test]
        public void StringWithoutControlCharactersIsReturnedWhole()
        {
            var data = "simple test string";
            var colourControlItems = ColourControlSplitter.Split(data).Select(t => t.ToString());
            var expected = new [] {new ColourControlItem(data).ToString()};
            Assert.That(colourControlItems, Is.EqualTo(expected));
        }

        [Test]
        public void NewlineIsExtractedAsAnInstruction()
        {
            var data = Environment.NewLine + "simple test" + Environment.NewLine + "string";
            var colourControlItems = ColourControlSplitter.Split(data).Select(t => t.ToString());
            var newLineControlItem = new ColourControlItem(instructions: new [] {new ColourControlItem.ControlInstruction(ColourControlItem.ControlCode.NewLine)});
            var expected = new []
            {
                newLineControlItem.ToString(),
                new ColourControlItem("simple test").ToString(), 
                newLineControlItem.ToString(),
                new ColourControlItem("string").ToString(), 
            };
            Assert.That(colourControlItems, Is.EqualTo(expected));
        }

        [Test]
        public void StringWithControlCharactersIsReturned()
        {
            var data = "simple test string".Blue();
            var output = string.Join(Environment.NewLine, ColourControlSplitter.Split(data).Select(t => t.ToString()));

            Approvals.Verify(output);
        }

        [Test]
        public void StringWithMultipleControlCharactersIsReturned()
        {
            var data = "simple test string".Yellow().BGCyan();
            var output = string.Join(Environment.NewLine, ColourControlSplitter.Split(data).Select(t => t.ToString()));

            Approvals.Verify(output);
        }

        [Test]
        public void ComplexStringIsReturned()
        {
            var data = "More complicated".Red() + " test".Green().BGCyan() + " string".Yellow().BGRed();
            var output = string.Join(Environment.NewLine, ColourControlSplitter.Split(data).Select(t => t.ToString()));

            Approvals.Verify(output);
        }
        
    }
}