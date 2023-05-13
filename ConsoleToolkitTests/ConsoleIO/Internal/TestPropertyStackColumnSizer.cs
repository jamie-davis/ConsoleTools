using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestPropertyStackColumnSizer
    {
        private PropertyStackColumnSizer _sizer;
        private List<PropertyColumnFormat> _columnFormats;

        class TestType
        {
            public string StringVal { get; set; }
            public string StringVal2 { get; set; }
        }
        public TestPropertyStackColumnSizer()
        {
            _sizer = new PropertyStackColumnSizer();
            _columnFormats = FormatAnalyser.Analyse(typeof(TestType), null, true).ToList();
        }

        [Fact]
        public void SizerWithNoContentHasZeroMinWidth()
        {
            Assert.Equal(0, _sizer.GetMinWidth());
        }

        [Fact]
        public void MinWidthIsCalculated()
        {
            _sizer.AddColumn(_columnFormats[0], new FormattingIntermediate[] { "A", "B", "CCCC" });
            Assert.Equal("String Val: CCCC".Length, _sizer.GetMinWidth());
        }

        [Fact]
        public void MinWidthIsLengthOfLongestWordInStackIfAllFirstLinesAreShorter()
        {
            const string longB = "BBBBBBBBBBBBBBBBBB";
            _sizer.AddColumn(_columnFormats[0], new FormattingIntermediate[] { "A", "B B B B B B " + longB + " B B B B B B B", "CCCC" });
            Assert.Equal(longB.Length, _sizer.GetMinWidth());
        }

        [Fact]
        public void MinWidthIsMinWidthOfLongestColumnInStack()
        {
            const string longB = "BBBBBBBBBBBBBBBBBB";
            _sizer.AddColumn(_columnFormats[0], new FormattingIntermediate[] { "A", "B B B B B B", "CCCC" });
            _sizer.AddColumn(_columnFormats[1], new FormattingIntermediate[] { "A", "B B B B B B " + longB + " B B B B B B B", "CCCC" });
            Assert.Equal(longB.Length, _sizer.GetMinWidth());
        }

        [Fact]
        public void MinWidthIsMinWidthOfLongestFirstLineInStack()
        {
            _sizer.AddColumn(_columnFormats[0], new FormattingIntermediate[] { "A", "BBBBBB", "CCCC" });
            _sizer.AddColumn(_columnFormats[1], new FormattingIntermediate[] { "A", "B", "CCCC" });
            Assert.Equal("String Val: BBBBBB".Length, _sizer.GetMinWidth());
        }

        [Fact]
        public void SizingValuesCanBeRetrieved()
        {
            const string longB = "BBBBBBBBBBBBBBBBBB";
            var colValues0 = new FormattingIntermediate[] { "A", "B B B B B B", "CCCC" };
            var colValues1 = new FormattingIntermediate[] { "A", "B B B B B B " + longB + " B B B B B B B", "CCCC" };
            _sizer.AddColumn(_columnFormats[1], colValues1);
            _sizer.AddColumn(_columnFormats[0], colValues0);

            Assert.Equal(new[] { colValues0[1], colValues1[1] }, _sizer.GetSizeValues(1));
        }

        [Fact]
        public void SizerWithRenderableReturnsMinWidth()
        {
            var colValues = new []
            {
                MakeRenderer("Some simple text", 3),
                MakeRenderer("Short", 1),
                MakeRenderer("Rather longer text that may need to be wrapped.", 5),
            };
            _sizer.AddColumn(_columnFormats[0], colValues);
            Assert.Equal("wrapped.".Length, _sizer.GetMinWidth());
        }

        private static FormattingIntermediate MakeRenderer(string text, int rows)
        {
            var recorder = new RecordingConsoleAdapter();
            recorder.WriteLine(text);
            recorder.FormatTable(Enumerable.Range(0, rows).Select(i => new {Index = i}));
            return new FormattingIntermediate(recorder);
        }
    }
}