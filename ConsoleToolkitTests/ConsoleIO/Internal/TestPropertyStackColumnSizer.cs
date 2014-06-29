using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
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

        [SetUp]
        public void SetUp()
        {
            _sizer = new PropertyStackColumnSizer();
            _columnFormats = FormatAnalyser.Analyse(typeof (TestType), null).ToList();
        }

        [Test]
        public void SizerWithNoContentHasZeroMinWidth()
        {
            Assert.That(_sizer.GetMinWidth(), Is.EqualTo(0));
        }

        [Test]
        public void MinWidthIsCalculated()
        {
            _sizer.AddColumn(_columnFormats[0], new FormattingIntermediate[] { "A", "B", "CCCC" });
            Assert.That(_sizer.GetMinWidth(), Is.EqualTo("String Val: CCCC".Length));
        }

        [Test]
        public void MinWidthIsLengthOfLongestWordInStackIfAllFirstLinesAreShorter()
        {
            const string longB = "BBBBBBBBBBBBBBBBBB";
            _sizer.AddColumn(_columnFormats[0], new FormattingIntermediate[] { "A", "B B B B B B " + longB + " B B B B B B B", "CCCC" });
            Assert.That(_sizer.GetMinWidth(), Is.EqualTo(longB.Length));
        }

        [Test]
        public void MinWidthIsMinWidthOfLongestColumnInStack()
        {
            const string longB = "BBBBBBBBBBBBBBBBBB";
            _sizer.AddColumn(_columnFormats[0], new FormattingIntermediate[] { "A", "B B B B B B", "CCCC" });
            _sizer.AddColumn(_columnFormats[1], new FormattingIntermediate[] { "A", "B B B B B B " + longB + " B B B B B B B", "CCCC" });
            Assert.That(_sizer.GetMinWidth(), Is.EqualTo(longB.Length));
        }

        [Test]
        public void MinWidthIsMinWidthOfLongestFirstLineInStack()
        {
            _sizer.AddColumn(_columnFormats[0], new FormattingIntermediate[] { "A", "BBBBBB", "CCCC" });
            _sizer.AddColumn(_columnFormats[1], new FormattingIntermediate[] { "A", "B", "CCCC" });
            Assert.That(_sizer.GetMinWidth(), Is.EqualTo("String Val: BBBBBB".Length));
        }

        [Test]
        public void SizingValuesCanBeRetrieved()
        {
            const string longB = "BBBBBBBBBBBBBBBBBB";
            var colValues0 = new FormattingIntermediate[] { "A", "B B B B B B", "CCCC" };
            var colValues1 = new FormattingIntermediate[] { "A", "B B B B B B " + longB + " B B B B B B B", "CCCC" };
            _sizer.AddColumn(_columnFormats[1], colValues1);
            _sizer.AddColumn(_columnFormats[0], colValues0);

            Assert.That(_sizer.GetSizeValues(1), Is.EqualTo(new [] {colValues0[1], colValues1[1]}));
        }

        [Test]
        public void SizerWithRenderableReturnsMinWidth()
        {
            var colValues = new []
            {
                MakeRenderer("Some simple text", 3),
                MakeRenderer("Short", 1),
                MakeRenderer("Rather longer text that may need to be wrapped.", 5),
            };
            _sizer.AddColumn(_columnFormats[0], colValues);
            Assert.That(_sizer.GetMinWidth(), Is.EqualTo("wrapped.".Length));
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