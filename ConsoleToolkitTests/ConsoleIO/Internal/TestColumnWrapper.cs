using System;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (DiffReporter))]
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
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20)));
            Assert.That(addedBreaks, Is.EqualTo(2));
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

        private string FormatResult(string value, string[] wrapped, int guideWidth)
        {
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
            sb.AppendLine(guide);
            sb.AppendLine(divide);

            foreach (var line in wrapped)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}