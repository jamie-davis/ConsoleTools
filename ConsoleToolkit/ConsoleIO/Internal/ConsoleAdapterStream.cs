using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// This class implements a text writer that writes text to a console adapter. This class is intended to be
    /// attached to the default console as the Out stream.
    /// </summary>
    internal class ConsoleAdapterStream : TextWriter
    {
        private ConsoleAdapter _adapter;

        public ConsoleAdapterStream(ConsoleAdapter adapter)
        {
            _adapter = adapter;
        }

        public override Encoding Encoding
        {
            get { return _adapter.GetEncoding(); }
        }

        public override void Write(string value)
        {
            _adapter.Write(value);
        }

        public override void Write(char value)
        {
            _adapter.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void WriteLine(string value)
        {
            _adapter.WriteLine(value);
        }

        public override void WriteLine(decimal value)
        {
            _adapter.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void WriteLine(double value)
        {
            _adapter.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void WriteLine(float value)
        {
            _adapter.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void WriteLine(int value)
        {
            _adapter.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void WriteLine(uint value)
        {
            _adapter.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void WriteLine(long value)
        {
            _adapter.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void WriteLine(ulong value)
        {
            _adapter.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void WriteLine(bool value)
        {
            _adapter.WriteLine(value.ToString());
        }

        public override void WriteLine(char value)
        {
            _adapter.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void  WriteLine(char[] buffer)
        {
            WriteLine(buffer, 0, buffer.Length);
        }

        public override void  WriteLine(char[] buffer, int index, int count)
        {
            var end = Math.Min(index + count, buffer.Length);
            var sb = new StringBuilder();
            while (index < end)
                sb.Append(buffer[index++].ToString(CultureInfo.InvariantCulture));
            WriteLine(sb.ToString());
        }
    }
}
