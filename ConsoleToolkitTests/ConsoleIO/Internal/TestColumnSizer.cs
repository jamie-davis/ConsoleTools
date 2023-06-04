using System;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestColumnSizer
    {

        [Fact]
        public void ZeroLineBreaksReturnsWidestLength()
        {
            var sizer = new ColumnSizer(typeof(int));
            sizer.ColumnValue("N");
            for (var n = 0; n <= 100; ++n)
                sizer.ColumnValue(n.ToString());

            Assert.Equal(3, sizer.MinWidth(0));
        }

        [Fact]
        public void IntDataDoesNotAllowLineBreaks()
        {
            var sizer = new ColumnSizer(typeof(int));
            sizer.ColumnValue("N");
            for (var n = 0; n <= 100; ++n)
                sizer.ColumnValue(n.ToString());

            Assert.Equal(3, sizer.MinWidth(10));
        }

        [Fact]
        public void DecimalDataDoesNotAllowLineBreaks()
        {
            var sizer = new ColumnSizer(typeof(decimal));
            sizer.ColumnValue("N");
            for (decimal n = 0; n <= 100; ++n)
                sizer.ColumnValue(n.ToString());

            Assert.Equal(3, sizer.MinWidth(10));
        }

        [Fact]
        public void DateTimeDataDoesNotAllowLineBreaks()
        {
            var sizer = new ColumnSizer(typeof(double));
            sizer.ColumnValue("Date");
            for (var n = 0; n <= 100; ++n)
                sizer.ColumnValue((DateTime.Parse("2014-04-28") + new TimeSpan(n, 0, 0, 0)).ToString("yyyy-MM-dd"));

            Assert.Equal(10, sizer.MinWidth(10));
        }

        [Fact]
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

            Assert.Equal(33, minWidth);
        }

        [Fact]
        public void FittingToLineBreaksStopsIfTheMaximumPossibleNumberOfLineBreaksIsReached()
        {
            var sizer = new ColumnSizer(typeof(string));
            sizer.ColumnValue("X");
            //                        ----+----|----+----|----+----|---\----+----|----+----|----+----|---\----+----|----+----|----+----|
            const string testValue = "A few words.";
            sizer.ColumnValue(testValue);

            var minWidth = sizer.MinWidth(200);
            Assert.Equal(1, minWidth);
        }

        [Fact]
        public void IdealMinWidthIsCalculated()
        {
            var sizer = new ColumnSizer(typeof(string));
            sizer.ColumnValue("XXXX XXXX");
            sizer.ColumnValue("YYYYYY XXXXX");
            
            Assert.Equal(6, sizer.GetIdealMinimumWidth());
        }

        [Fact]
        public void IdealMinWidthIsReCalculated()
        {
            var sizer = new ColumnSizer(typeof(string));
            sizer.ColumnValue("XXXX XXXX");
            sizer.ColumnValue("YYYYYY XXXXX");

            var oldWidth = sizer.GetIdealMinimumWidth();
            sizer.ColumnValue("YYYYYYY");

            Assert.Equal(7, sizer.GetIdealMinimumWidth());
        }

        [Fact]
        public void FixedColumnIdealMinWidthIsAlwaysFixed()
        {
            var sizer = new ColumnSizer(typeof(string), new ColumnFormat(type: typeof(string))  { FixedWidth = 4});
            sizer.ColumnValue("XXXX XXXX");
            sizer.ColumnValue("YYYYYY XXXXX");

            var oldWidth = sizer.GetIdealMinimumWidth();
            sizer.ColumnValue("YYYYYYY");

            Assert.Equal(4, sizer.GetIdealMinimumWidth());
        }

        [Fact]
        public void FixedColumnMinWidthIsAlwaysFixed()
        {
            var sizer = new ColumnSizer(typeof(string), new ColumnFormat(type: typeof(string))  { FixedWidth = 4});
            sizer.ColumnValue("XXXX XXXX");
            sizer.ColumnValue("YYYYYY XXXXX");

            var width = sizer.MinWidth(0);
            Assert.Equal(4, width);
        }

        [Fact]
        public void MinWidthColumnIdealMinWidthIsAlwaysMinimum()
        {
            var sizer = new ColumnSizer(typeof(string), new ColumnFormat(type: typeof(string))  { MinWidth = 6});
            sizer.ColumnValue("XXXX XXXX");
            sizer.ColumnValue("YYYYYY XXXXX");

            var oldWidth = sizer.GetIdealMinimumWidth();
            sizer.ColumnValue("YYYYYYY");

            Assert.Equal(6, sizer.GetIdealMinimumWidth());
        }

        [Fact]
        public void MinWidthColumnMinWidthIsAlwaysMinimum()
        {
            var sizer = new ColumnSizer(typeof(string), new ColumnFormat(type: typeof(string)) { MinWidth = 6 });
            sizer.ColumnValue("XXXX XXXX");
            sizer.ColumnValue("YYYYYY XXXXX");

            var width = sizer.MinWidth(0);
            Assert.Equal(6, width);
        }

        [Fact]
        public void RenderableColumnValuesAreNotConvertedToText()
        {
            var sizer = new ColumnSizer(typeof(string));
            sizer.ColumnValue("XXXX XXXX");

            //add a renderable value
            var renderable = new RecordingConsoleAdapter();
            renderable.FormatTable(Enumerable.Range(0, 3).Select(i => new {String = "blah blah blah blah", Number = i}));
            sizer.ColumnValue(renderable);

            sizer.GetSizeValue(1).RenderableValue.Should().NotBeNull();
        }

        [Fact]
        public void MaxLineBreaksIsCalculated()
        {
            var columnFormat = new ColumnFormat("", typeof(string));
            var sizer = new ColumnSizer(typeof(string), columnFormat);
            sizer.ColumnValue("XXXX XXXX XXXX XX XXX");
            sizer.ColumnValue("YYYYYY YYYYY YY YYY YY YYYY YY Y");

            var sb = new StringBuilder();
            sb.AppendLine("Test values:");
            sb.AppendLine(sizer.GetSizeValue(0).TextValue);
            sb.AppendLine(sizer.GetSizeValue(1).TextValue);

            sb.AppendLine("Max Linebreaks:");
            for (var width = 15; width > 0; --width)
            {
                Console.WriteLine(width);
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