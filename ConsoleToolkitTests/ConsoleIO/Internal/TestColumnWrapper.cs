using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
using Approvals = ApprovalTests.Approvals;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestColumnWrapper
    {
        [Test]
        public void StringIsWrappedAtWordBreaks()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four five six seven eight nine ten eleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void NoTrailingSpacesAreIncludedOnLineWrap()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four\t\t\t\t\t\t\t\t          five\tsix seven eight nine ten eleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void WordsLongerThanOneLineAreBroken()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four fivesixseveneightnineteneleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void VeryLongInitialWordisBroken()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "Onetwothreefourfivesixseveneightnineten eleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void LineBreaksArePreserved()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three\r\nfour five six seven eight\r\n\r\n\r\nnine\r\nten\r\neleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void ColourInstructionsAreIncluded()
        {
            var c = new ColumnFormat("h", typeof (string));
            var value = "One".Red() +" " + "two".Blue() + " three\r\nfour five six seven eight\r\n\r\n\r\nnine\r\nten\r\neleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void LineBreaksWithinColouredTextAreFormatted()
        {
            var c = new ColumnFormat("h", typeof (string));
            var value = "One\r\ntwo\r\nthree".Red().BGBlue();
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void ColouredSpacesAreSkippedAtTheEndOfTheLineButInstructionsArePreserved()
        {
            var c = new ColumnFormat("h", typeof (string));
            var value = "four five six seven" + " ".Red() + " ".White() + " ".Blue() + "eight";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void ColourInstructionsForLastWordAreIncluded()
        {
            var c = new ColumnFormat("h", typeof (string));
            var value = "four five six seven " + "eight".Red();
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void WordBreaksAreCounted()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four five six seven eight nine ten eleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20)));
            Assert.That(addedBreaks, Is.EqualTo(2));
        }

        [Test]
        public void TrailingSpacesDoNotCauseAdditionalLineBreaks()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four\t\t\t\t\t\t\t\t          five\tsix seven eight nine ten eleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20);
            Console.WriteLine("----+----|----+----|");
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20)));
            Assert.That(addedBreaks, Is.EqualTo(3));
        }

        [Test]
        public void BreaksInVeryLongWordsAreCounted()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four fivesixseveneightnineteneleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20)));
            Assert.That(addedBreaks, Is.EqualTo(2));
        }

        [Test]
        public void BreaksInVeryLongInitialWordAreCounted()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "Onetwothreefourfivesixseveneightnineten eleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20)));
            Assert.That(addedBreaks, Is.EqualTo(2));
        }

        [Test]
        public void DataLineBreaksAreNotCounted()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three\r\nfour five six seven eight\r\n\r\n\r\nnine\r\nten\r\neleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20)));
            Assert.That(addedBreaks, Is.EqualTo(1));
        }

        [Test]
        public void SingleLineDataAddsNoLineBreaks()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20)));
            Assert.That(addedBreaks, Is.EqualTo(0));
        }

        [Test]
        public void NestedColourChangesArePreserved()
        {
            var c = new ColumnFormat("h", typeof (string));
            var value = (("Red" + Environment.NewLine + "Lines").Cyan() + Environment.NewLine + "lines").BGDarkRed() + "Clear";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void LinesWithColourAreExpandedToCorrectLength()
        {
            var c = new ColumnFormat("h", typeof (string));
            c.SetActualWidth(20);
            var value = (("Red" + Environment.NewLine + "Lines").Cyan() + Environment.NewLine + "lines").BGDarkRed() + "Clear";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20).Select(l => "-->" + l + "<--");
            var result = FormatResult(value, wrapped, 20, 3);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void LinesAreExpandedToCorrectLength()
        {
            var c = new ColumnFormat("h", typeof (string));
            c.SetActualWidth(20);
            var value = "Line" + Environment.NewLine + "Data" + Environment.NewLine + "More";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20).Select(l => "-->" + l + "<--");
            var result = FormatResult(value, wrapped, 20, 3);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void RightAlignedLinesAreExpandedToCorrectLength()
        {
            var c = new ColumnFormat("h", typeof (string), ColumnAlign.Right);
            c.SetActualWidth(20);
            var value = "Line" + Environment.NewLine + "Data" + Environment.NewLine + "More";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20).Select(l => "-->" + l + "<--");
            var result = FormatResult(value, wrapped, 20, 3);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void RightAlignedLinesWithColourAreExpandedToCorrectLength()
        {
            var c = new ColumnFormat("h", typeof (string), ColumnAlign.Right);
            c.SetActualWidth(20);
            var value = (("Red" + Environment.NewLine + "Lines").Cyan() + Environment.NewLine + "lines").BGDarkRed() + "Clear";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20).Select(l => "-->" + l + "<--");
            var result = FormatResult(value, wrapped, 20, 3);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void RightAlignedLinesWithExcessiveColumnWidthAreExpandedToCorrectLength()
        {
            var c = new ColumnFormat("h", typeof (string), ColumnAlign.Right);
            c.SetActualWidth(20);
            var value = (("Red" + Environment.NewLine + "Lines").Cyan() + Environment.NewLine + "lines").BGDarkRed() + "Clear";
            var wrapped = ColumnWrapper.WrapValue(value, c, 10).Select(l => "-->" + l + "<--");
            var result = FormatResult(value, wrapped, 10, 3);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void ColouredWordsTooLongForALineAreCorrectlyChunked()
        {
            var c = new ColumnFormat("h", typeof (string));
            var value = "toomuchdataforeventwolines".Red();
            var wrapped = ColumnWrapper.WrapValue(value, c, 9);
            var result = FormatResult(value, wrapped, 10);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void FirstLineIndentShortensFirstLine()
        {
            var c = new ColumnFormat("h", typeof(string));
            const string value = "One two three four five six seven eight nine ten eleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, firstLineHangingIndent: 10);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void LeadingSpacesArePreserved()
        {
            var c = new ColumnFormat("h", typeof(string));
            const string value = " One two three.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void TrailingSpacesArePreserved()
        {
            var c = new ColumnFormat("h", typeof(string));
            const string value = "One two three. ";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void WrapAndMeasureWordsCanWorkWithSplitData()
        {
            //Arrange
            var c = new ColumnFormat("h", typeof(string));
            const string value = "One two three four five six seven eight nine ten eleven.";
            var words = WordSplitter.SplitToList(value, 4);

            //Act
            int wrappedLines;
            var wrapped = ColumnWrapper.WrapAndMeasureWords(words, c, 20, 0, out wrappedLines);

            //Assert
            var result = FormatResult(value, wrapped, 20)
                + Environment.NewLine
                + string.Format("Added line breaks = {0}", wrappedLines);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        private string FormatResult(string value, IEnumerable<string> wrapped, int guideWidth, int indent = 0)
        {
            var indentString = new string(' ', indent);
            var sb = new StringBuilder();
            sb.AppendLine(TestContext.CurrentContext.Test.Name);
            sb.AppendLine();

            sb.AppendLine("Original:");
            sb.AppendLine(value);
            sb.AppendLine();

            sb.AppendLine("Wrapped:");
            var guide = Enumerable.Range(0, guideWidth)
                .Select(i => ((i + 1)%10).ToString())
                .Aggregate((t, i) => t + i);
            var divide = Enumerable.Range(0, guideWidth)
                .Select(i => ((i + 1) % 10) == 0 ? "+" : "-")
                .Aggregate((t, i) => t + i);
            sb.AppendLine(indentString + guide);
            sb.AppendLine(indentString + divide);

            foreach (var line in wrapped)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}