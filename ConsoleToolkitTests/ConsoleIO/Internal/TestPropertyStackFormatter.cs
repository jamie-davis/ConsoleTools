using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestPropertyStackFormatter
    {
        private static readonly string End = Environment.NewLine + "End";
        const int ColumnWidth = 20;

        [Fact]
        public void FormatHeadingIsUsedAsThePropertyName()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading"), "Value", ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Fact]
        public void LongValuesAreWordWrapped()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading"), "A nice long value that can only be accomodated with word wrapping.", ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Fact]
        public void AllPartsOfARightAlignedValueAreRightAligned()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading", alignment: ColumnAlign.Right), "A nice long value that can only be accomodated with word wrapping.", ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Fact]
        public void FormattingAcceptsValueWhereFirstWordDoesNotFitOnFirstLine()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Format heading", alignment: ColumnAlign.Right), "Firstwordislong", ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Fact]
        public void FirstLineHangingIndentIsRespected()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Format heading", alignment: ColumnAlign.Right), "Value words words", ColumnWidth, firstLineHangingIndent: 10);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Fact]
        public void RenderableElementsAreStacked()
        {
            var recorder = new RecordingConsoleAdapter();
            recorder.WriteLine("Some simple text");
            recorder.FormatTable(Enumerable.Range(0,5).Select(i => new {Index = i}));
            var output = PropertyStackFormatter.Format(new ColumnFormat("Format heading", 
                alignment: ColumnAlign.Right), 
                recorder, 
                ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }
    }
}