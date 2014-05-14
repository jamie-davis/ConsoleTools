using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestDefaultWidthCalculator
    {
        private List<ColumnFormat> _formats;

        [SetUp]
        public void SetUp()
        {
            _formats = new List<ColumnFormat>
            {
                MakeFormat<sbyte>(),
                MakeFormat<byte>(),
                MakeFormat<short>(),
                MakeFormat<ushort>(),
                MakeFormat<int>(),
                MakeFormat<uint>(),
                MakeFormat<long>(),
                MakeFormat<ulong>(),
                MakeFormat<decimal>(),
                MakeFormat<float>(),
                MakeFormat<double>(),
                MakeFormat<string>(),
                MakeFormat<char>(),
                MakeFormat<DateTime>(),
                MakeFormat<TimeSpan>(),
                MakeFormat<bool>(),
                MakeFormat<object>(),
            };
        }

        private ColumnFormat MakeFormat<T>()
        {
            return new ColumnFormat("A", typeof(T));
        }

        [Test]
        public void DefaultMinWidthIsAsExpected()
        {
            var report = AllFormats(f => DefaultWidthCalculator.Min(f).ToString(CultureInfo.InvariantCulture));
            Approvals.Verify(report);
        }

        [Test]
        public void DefaultMaxWidthIsAsExpected()
        {
            var report = AllFormats(f => DefaultWidthCalculator.Max(f).ToString(CultureInfo.InvariantCulture));
            Approvals.Verify(report);
        }

        private string AllFormats(Func<ColumnFormat, string> func)
        {
            var report = string.Join(Environment.NewLine,
                _formats.Select(f => string.Format("{0,-8} = {1}", f.Type.Name, func(f))));
            Console.WriteLine(report);
            return report;
        }
    }
}