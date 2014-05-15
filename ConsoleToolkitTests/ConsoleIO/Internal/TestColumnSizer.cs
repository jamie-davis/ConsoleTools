using System;
using System.Text;
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
    public class TestColumnSizer
    {

        [Test]
        public void ZeroLineBreaksReturnsWidestLength()
        {
            var sizer = new ColumnSizer(typeof(int));
            sizer.ColumnValue("N");
            for (var n = 0; n <= 100; ++n)
                sizer.ColumnValue(n.ToString());

            Assert.That(sizer.MinWidth(0), Is.EqualTo(3));
        }

        [Test]
        public void IntDataDoesNotAllowLineBreaks()
        {
            var sizer = new ColumnSizer(typeof(int));
            sizer.ColumnValue("N");
            for (var n = 0; n <= 100; ++n)
                sizer.ColumnValue(n.ToString());

            Assert.That(sizer.MinWidth(10), Is.EqualTo(3));
        }

        [Test]
        public void DecimalDataDoesNotAllowLineBreaks()
        {
            var sizer = new ColumnSizer(typeof(decimal));
            sizer.ColumnValue("N");
            for (decimal n = 0; n <= 100; ++n)
                sizer.ColumnValue(n.ToString());

            Assert.That(sizer.MinWidth(10), Is.EqualTo(3));
        }

        [Test]
        public void DateTimeDataDoesNotAllowLineBreaks()
        {
            var sizer = new ColumnSizer(typeof(double));
            sizer.ColumnValue("Date");
            for (var n = 0; n <= 100; ++n)
                sizer.ColumnValue((DateTime.Parse("2014-04-28") + new TimeSpan(n, 0, 0, 0)).ToString("yyyy-MM-dd"));

            Assert.That(sizer.MinWidth(10), Is.EqualTo(10));
        }

        [Test]
        public void StringValuesAllowLineBreaks()
        {
            var sizer = new ColumnSizer(typeof(string));
            sizer.ColumnValue("X");
            //                        ----+----|----+----|----+----|---\----+----|----+----|----+----|---\----+----|----+----|----+----|
            const string testValue = "Several words so that line breaks can be added to fit a small column to a number of lines.";
            sizer.ColumnValue(testValue);

            var minWidth = sizer.MinWidth(2);

            var formatted = ColumnWrapper.WrapValue(testValue, new ColumnFormat("X", typeof (string)), minWidth);
            Console.WriteLine(RulerFormatter.MakeRuler(minWidth));
            Console.WriteLine(string.Join(Environment.NewLine, formatted));

            Assert.That(minWidth, Is.EqualTo(33));
        }

        [Test]
        public void FittingToLineBreaksStopsIfTheMaximumPossibleNumberOfLineBreaksIsReached()
        {
            var sizer = new ColumnSizer(typeof(string));
            sizer.ColumnValue("X");
            //                        ----+----|----+----|----+----|---\----+----|----+----|----+----|---\----+----|----+----|----+----|
            const string testValue = "A few words.";
            sizer.ColumnValue(testValue);

            var minWidth = sizer.MinWidth(200);
            Assert.That(minWidth, Is.EqualTo(1));
        }

        [Test]
        public void IdealMinWidthIsCalculated()
        {
            var sizer = new ColumnSizer(typeof(string));
            sizer.ColumnValue("XXXX XXXX");
            sizer.ColumnValue("YYYYYY XXXXX");
            
            Assert.That(sizer.GetIdealMinimumWidth(), Is.EqualTo(6));
        }

        [Test]
        public void IdealMinWidthIsReCalculated()
        {
            var sizer = new ColumnSizer(typeof(string));
            sizer.ColumnValue("XXXX XXXX");
            sizer.ColumnValue("YYYYYY XXXXX");

            var oldWidth = sizer.GetIdealMinimumWidth();
            sizer.ColumnValue("YYYYYYY");

            Assert.That(sizer.GetIdealMinimumWidth(), Is.EqualTo(7));
        }

        [Test]
        public void MaxLineBreaksIsCalculated()
        {
            var columnFormat = new ColumnFormat("", typeof(string));
            var sizer = new ColumnSizer(typeof(string), columnFormat);
            sizer.ColumnValue("XXXX XXXX XXXX XX XXX");
            sizer.ColumnValue("YYYYYY YYYYY YY YYY YY YYYY YY Y");

            var sb = new StringBuilder();
            sb.AppendLine("Test values:");
            sb.AppendLine(sizer.GetSizeValue(0));
            sb.AppendLine(sizer.GetSizeValue(1));

            sb.AppendLine("Max Linebreaks:");
            for (var width = 15; width > 0; --width)
            {
                sb.AppendLine();
                sb.AppendFormat("Width = {0}, line breaks = {1}", width, sizer.GetMaxLineBreaks(width));
                sb.AppendLine();
                sb.AppendLine(RulerFormatter.MakeRuler(width));
                foreach (var line in ColumnWrapper.WrapValue(sizer.GetSizeValue(1), columnFormat, width))
                {
                    sb.AppendLine(line);
                }
            }

            Console.WriteLine(sb.ToString());
            Approvals.Verify(sb.ToString());
        }
    }
}