using System;
using System.Globalization;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
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
        public TestFormattingIntermediate()
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

        [Fact]
        public void LongestWordIsCalculatedForStrings()
        {
            Assert.Equal("measuring".Length, _simpleValue.GetLongestWordLength(4));
        }

        [Fact]
        public void LengthOfFirstLineIsCalculatedForStrings()
        {
            Assert.Equal("Simple".Length, _multiLineValue.GetFirstWordLength(4, 0));
        }

        [Fact]
        public void HangingIndentIsIncludedInFirstLineOfStrings()
        {
            Assert.Equal("Simple".Length + 5, _multiLineValue.GetFirstWordLength(4, 5));
        }

        [Fact]
        public void LongestWordIsCalculatedForRenderable()
        {
            Assert.Equal("measuring".Length, _renderableValue.GetLongestWordLength(4));
        }

        [Fact]
        public void FirstWordLengthIsCalculatedForRenderable()
        {
            Assert.Equal("Simple".Length, _renderableValue.GetFirstWordLength(4, 0));
        }

        [Fact]
        public void LongestWordIsCalculatedForRenderableTable()
        {
            Assert.Equal("Index three".Length, _renderableTableValue.GetLongestWordLength(4));
        }

        [Fact]
        public void FirstWordLengthIsCalculatedForRenderableTable()
        {
            Assert.Equal("Index three".Length, _renderableTableValue.GetFirstWordLength(4, 0));
        }

        [Fact]
        public void WidthIsTakenFromString()
        {
            Assert.Equal(56, _simpleValue.Width);
        }

        [Fact]
        public void WidthIsLongestWidthFromRenderable()
        {
            Assert.Equal("Index three".Length, _renderableTableValue.Width);
        }
    }
}