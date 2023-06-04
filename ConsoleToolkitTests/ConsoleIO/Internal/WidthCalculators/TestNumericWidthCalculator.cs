using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Internal.WidthCalculators;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal.WidthCalculators
{
    [UseReporter(typeof (CustomReporter))]
    public class TestNumericWidthCalculator
    {
        private List<ColumnFormat> _formats;
        private List<ColumnFormat> _templatedFormats;
        public TestNumericWidthCalculator()
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
            };

            _templatedFormats = new List<ColumnFormat>
            {
                MakeFormat<sbyte>("0.000"),
                MakeFormat<byte>("0.000"),
                MakeFormat<short>("0.000"),
                MakeFormat<ushort>("0.000"),
                MakeFormat<int>("0.000"),
                MakeFormat<uint>("0.000"),
                MakeFormat<long>("0.000"),
                MakeFormat<ulong>("0.000"),
                MakeFormat<decimal>("0.000"),
                MakeFormat<float>("0.000"),
                MakeFormat<double>("0.000"),
            };
        }

        private ColumnFormat MakeFormat<T>(string template = null)
        {
            if (template == null)
                return new ColumnFormat("A", typeof (T));

            return new ColumnFormat("A", typeof(T), format: template);
        }

        [Fact]
        public void WithoutTemplatesMinAndMaxAreSetToWidestForType()
        {
            var reportLines =
                _formats.Select(f => string.Format("{0,-8} = {1} - {2}", f.Type.Name, 
                                                    NumericWidthCalculator.Min(f),
                                                    NumericWidthCalculator.Max(f)));
            var output = string.Join(Environment.NewLine, reportLines);
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

        [Fact]
        public void WithTemplatesMinAndMaxAreSetToWidestForType()
        {
            var reportLines =
                _templatedFormats.Select(f => string.Format("{0,-8} = {1} - {2}", f.Type.Name, 
                                                    NumericWidthCalculator.Min(f),
                                                    NumericWidthCalculator.Max(f)));
            var output = string.Join(Environment.NewLine, reportLines);
            Console.WriteLine(output);
            Approvals.Verify(output);
        }

    }
}
