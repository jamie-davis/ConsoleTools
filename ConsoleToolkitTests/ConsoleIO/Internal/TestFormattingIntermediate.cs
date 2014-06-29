using System;
using System.Globalization;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestFormattingIntermediate
    {
        private const string SimpleText = "Simple text string for the purposes of measuring things.";
        private FormattingIntermediate _simpleValue;
        private FormattingIntermediate _multiLineValue;
        private RecordingConsoleAdapter _renderable;
        private RecordingConsoleAdapter _renderableTable;
        private FormattingIntermediate _renderableValue;
        private FormattingIntermediate _renderableTableValue;

        [SetUp]
        public void SetUp()
        {
            _simpleValue = new FormattingIntermediate(SimpleText);
            _multiLineValue = new FormattingIntermediate("Simple text string" + Environment.NewLine +
                                                         "for the purposes of measuring things.");
            _renderable = new RecordingConsoleAdapter();
            RenderText();
            _renderableValue = new FormattingIntermediate(_renderable);

            _renderableTable = new RecordingConsoleAdapter();
            RenderTable();
            _renderableTableValue = new FormattingIntermediate(_renderableTable);
        }

        private void RenderText()
        {
            _renderable.WrapLine(SimpleText);
        }

        private void RenderTable()
        {
            string[] numbers = {"one", "two", "three", "four", "five"};
            var data = Enumerable.Range(0, 5).Select(i => new {Index = i, Text = numbers[i]});
            _renderableTable.FormatTable(data);
        }

        [Test]
        public void LongestWordIsCalculatedForStrings()
        {
            Assert.That(_simpleValue.GetLongestWordLength(4), Is.EqualTo("measuring".Length));
        }

        [Test]
        public void LengthOfFirstLineIsCalculatedForStrings()
        {
            Assert.That(_multiLineValue.GetFirstWordLength(4, 0), Is.EqualTo("Simple".Length));
        }

        [Test]
        public void HangingIndentIsIncludedInFirstLineOfStrings()
        {
            Assert.That(_multiLineValue.GetFirstWordLength(4, 5), Is.EqualTo("Simple".Length + 5));
        }

        [Test]
        public void LongestWordIsCalculatedForRenderable()
        {
            Assert.That(_renderableValue.GetLongestWordLength(4), Is.EqualTo("measuring".Length));
        }

        [Test]
        public void FirstWordLengthIsCalculatedForRenderable()
        {
            Assert.That(_renderableValue.GetFirstWordLength(4, 0), Is.EqualTo("Simple".Length));
        }

        [Test]
        public void LongestWordIsCalculatedForRenderableTable()
        {
            Assert.That(_renderableTableValue.GetLongestWordLength(4), Is.EqualTo("Index three".Length));
        }

        [Test]
        public void FirstWordLengthIsCalculatedForRenderableTable()
        {
            Assert.That(_renderableTableValue.GetFirstWordLength(4, 0), Is.EqualTo("Index three".Length));
        }

        [Test]
        public void WidthIsTakenFromString()
        {
            Assert.That(_simpleValue.Width, Is.EqualTo(56));
        }

        [Test]
        public void WidthIsLongestWidthFromRenderable()
        {
            Assert.That(_renderableTableValue.Width, Is.EqualTo("Index three".Length));
        }
    }
}