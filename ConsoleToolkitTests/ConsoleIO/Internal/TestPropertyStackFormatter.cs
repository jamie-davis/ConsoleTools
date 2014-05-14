using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestPropertyStackFormatter
    {
        private static readonly string End = Environment.NewLine + "End";
        const int ColumnWidth = 20;

        [Test]
        public void FormatHeadingIsUsedAsThePropertyName()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading"), "Value", ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Test]
        public void LongValuesAreWordWrapped()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading"), "A nice long value that can only be accomodated with word wrapping.", ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Test]
        public void AllPartsOfARightAlignedValueAreRightAligned()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading", alignment: ColumnAlign.Right), "A nice long value that can only be accomodated with word wrapping.", ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Test]
        public void FormattingAcceptsValueWhereFirstWordDoesNotFitOnFirstLine()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Format heading", alignment: ColumnAlign.Right), "Firstwordislong", ColumnWidth);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }
    }
}