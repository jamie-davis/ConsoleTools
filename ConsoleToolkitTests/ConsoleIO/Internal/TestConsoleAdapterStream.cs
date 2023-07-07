using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestConsoleAdapterStream
    {
        private ConsoleInterfaceForTesting _outInterface;
        private ConsoleAdapter _adapter;
        private ConsoleAdapterStream _stream;
        public TestConsoleAdapterStream()
        {
            _outInterface = new ConsoleInterfaceForTesting();
            _adapter = new ConsoleAdapter(_outInterface);
            _stream = new ConsoleAdapterStream(_adapter);
        }

        [Fact]
        public void TextIsWritten()
        {
            _stream.Write("blah");
            Approvals.Verify(_outInterface.GetBuffer());
        }

        public static Action<TextWriter>[] WriteTestArray = new Action<TextWriter>[]
        {
            t => t.Write(true),
            t => t.Write('c'),
            t => t.Write(new[] {'a', 'b', 'c', 'd'}),
            t => t.Write(new[] {'a', 'b', 'c', 'd'}, 1, 2),
            t => t.Write(decimal.MaxValue),
            t => t.Write(double.MaxValue),
            t => t.Write(float.MaxValue),
            t => t.Write(Int32.MaxValue),
            t => t.Write(Int64.MaxValue),
            t => t.Write(ulong.MaxValue),
            t => t.Write(uint.MaxValue),
            t => t.Write(byte.MaxValue),
            t => t.Write(sbyte.MaxValue),
            t => t.Write(new object()),
            t => t.Write("{0}", "first"),
            t => t.Write("{0},{1}", "first", "second"),
            t => t.Write("{0},{1},{2}", "first", "second", "third"),
            t => t.Write("{0},{1},{2},{3}", "first", "second", "third", "fourth"),
            t => t.Write("value"),
        };

        public static object[][] WriteTests = WriteTestArray.Select(a => new object[]{a}).ToArray();

        public static Action<TextWriter>[] WriteLineTestsArray = new Action<TextWriter>[]
        {
            t => t.WriteLine(true),
            t => t.WriteLine('q'),
            t => t.WriteLine(new[] {'a', 'b', 'c', 'd'}),
            t => t.WriteLine(new[] {'e', 'f', 'g', 'h'}, 1, 2),
            t => t.WriteLine(decimal.MaxValue),
            t => t.WriteLine(double.MaxValue),
            t => t.WriteLine(float.MaxValue),
            t => t.WriteLine(Int32.MaxValue),
            t => t.WriteLine(Int64.MaxValue),
            t => t.WriteLine(ulong.MaxValue),
            t => t.WriteLine(uint.MaxValue),
            t => t.WriteLine(byte.MaxValue),
            t => t.WriteLine(sbyte.MaxValue),
            t => t.WriteLine(new object()),
            t => t.WriteLine("{0}", "first"),
            t => t.WriteLine("{0},{1}", "first", "second"),
            t => t.WriteLine("{0},{1},{2}", "first", "second", "third"),
            t => t.WriteLine("{0},{1},{2},{3}", "first", "second", "third", "fourth"),
            t => t.WriteLine("value"),
        };
        public static object[][] WriteLineTests = WriteLineTestsArray.Select(a => new object[]{a}).ToArray();

        [Theory, MemberData(nameof(WriteTests))]
        public void TextWriteMethodsWork(Action<TextWriter> fn)
        {
            fn(_stream);
            fn(_stream);
            using (var writer = new StringWriter())
            {
                fn(writer);
                fn(writer);
                Assert.Equal(writer.ToString(), _outInterface.GetBuffer().TrimEnd());
            }
        }

        [Theory, MemberData(nameof(WriteLineTests))]
        public void TextWriteLineMethodsWork(Action<TextWriter> fn)
        {
            fn(_stream);
            fn(_stream);
            using (var writer = new StringWriter())
            {
                fn(writer);
                fn(writer);
                Assert.Equal(writer.ToString(), TrimLines(_outInterface.GetBuffer()));
            }
        }

        private string TrimLines(string lines)
        {
            var sb = new StringBuilder();
            var position = 0;
            var nextNl = 0;
            while ((nextNl = lines.IndexOf(Environment.NewLine, position, StringComparison.Ordinal)) >= 0)
            {
                var line = lines.Substring(position, nextNl - position);
                sb.AppendLine(line.TrimEnd());
                position = nextNl + Environment.NewLine.Length;
            }

            if (position < lines.Length)
            {
                var line = lines.Substring(position).TrimEnd();
                if (line.Length > 0)
                    sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}