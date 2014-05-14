using System;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestValueFormatter
    {
        class MyType
        {
            public override string ToString()
            {
                return "My type's value";
            }
        }

        class Formattable
        {
            public override string ToString()
            {
                return "My type's value";
            }

            public string ToString(string format)
            {
                return "Custom formatter called with " + format;
            }
        }


        [Test]
        public void DecimalValuesAreDisplayedToTwoDpByDefault()
        {
            var format = new ColumnFormat("decimal", type: typeof(Decimal));
            Assert.That(ValueFormatter.Format(format, 1.456m), Is.EqualTo("1.46"));
        }

        [Test]
        public void FloatValuesAreDisplayedToTwoDpByDefault()
        {
            var format = new ColumnFormat("float", type: typeof(float));
            Assert.That(ValueFormatter.Format(format, 1.456F), Is.EqualTo("1.46"));
        }

        [Test]
        public void DoubleValuesAreDisplayedToTwoDpByDefault()
        {
            var format = new ColumnFormat("double", type: typeof(float));
            Assert.That(ValueFormatter.Format(format, 1.456D), Is.EqualTo("1.46"));
        }

        [Test]
        public void FloatingPointNumbersAreFormattedToTheSpecifiedDp()
        {
            var formatters = new[]
            {
                new ColumnFormat("0 Dp", decimalPlaces: 0),
                new ColumnFormat("1 Dp", decimalPlaces: 1),
                new ColumnFormat("2 Dp", decimalPlaces: 2),
                new ColumnFormat("3 Dp", decimalPlaces: 3),
                new ColumnFormat("4 Dp", decimalPlaces: 4),
                new ColumnFormat("5 Dp", decimalPlaces: 5),
                new ColumnFormat("7 Dp", decimalPlaces: 7),
            };

            var values = new object[] { -1.23456m, -1.23456F, -1.23456D, -1, -2L, (short)-3, (sbyte)-4, (byte)5, DateTime.MinValue, "string", new TimeSpan(1) };

            string result = string.Empty;
            foreach (var columnFormat in formatters)
            {
                result += string.Format("{0}:", columnFormat.Heading) + Environment.NewLine;
                result += values
                    .Select(v => string.Format("    {0,-15} : {1}", v.GetType(), ValueFormatter.Format(columnFormat, v)))
                    .Aggregate((t, i) => t + Environment.NewLine + i);
                result += Environment.NewLine;
            }

            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void FormatStringIsApplied()
        {
            var data = new[]
            {
                new object[] {new ColumnFormat("num", format: "0.0000"), 100.123456m, 10000.2f, -45.1m, 1/3d, 99},
                new object[] { new ColumnFormat("date", format: "yyyy-MM-dd HH:mm:ss:ffff"), DateTime.Parse("2014-04-18 07:54:25.5987215")},
                new object[] { new ColumnFormat("span", format: "ddd"), new TimeSpan(14, 12, 2, 15)},
                new object[] { new ColumnFormat("other", format: "0.000"), "non formattable", 99, new MyType(), new Formattable()},
            };

            var sb = new StringBuilder();

            foreach (var dataArray in data)
            {
                var format = dataArray[0] as ColumnFormat;
                if (format == null)
                    sb.AppendLine("Bad data array, skipping.");
                else
                {
                    sb.AppendFormat("{0}:", format.FormatTemplate);
                    sb.AppendLine();
                    foreach (var item in dataArray.Skip(1))
                    {
                        sb.AppendFormat("    {0,-11} {1, 21} = {2}", item.GetType().Name, item, ValueFormatter.Format(format, item));
                        sb.AppendLine();
                    }
                }

                sb.AppendLine();
            }

            Console.WriteLine(sb);
            Approvals.Verify(sb.ToString());
        }
    }
}