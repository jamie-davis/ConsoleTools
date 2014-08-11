using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class wraps the actual console. Do not use the console directly, output text via the adapter instead. 
    /// This allows the console output to be captured in a unit test without requiring code changes.
    /// </summary>
    public class ConsoleAdapter : IConsoleAdapter
    {
        private static readonly ColumnFormat DefaultFormat = new ColumnFormat(null);

        private readonly IConsoleOutInterface _consoleOutInterface;
        private IConsoleInInterface _consoleInInterface;
        private Lazy<IConsoleInterface> _defaultConsole = new Lazy<IConsoleInterface>(MakeDefaultConsole);
        private readonly ColourWriter _writer;

        public ConsoleAdapter(IConsoleOutInterface consoleOutInterface = null, IConsoleInInterface consoleInInterface= null)
        {
            _consoleInInterface = consoleInInterface ?? _defaultConsole.Value;
            _consoleOutInterface = consoleOutInterface ?? _defaultConsole.Value;
            _writer = new ColourWriter(_consoleOutInterface);
        }

        public ConsoleAdapter(IConsoleInterface consoleInterface)
        {
            _consoleInInterface = consoleInterface ?? _defaultConsole.Value;
            _consoleOutInterface = consoleInterface ?? _defaultConsole.Value;
            _writer = new ColourWriter(_consoleOutInterface);
        }

        private static IConsoleInterface MakeDefaultConsole()
        {
            var redirectTester = new ConsoleRedirectTester();
            if (redirectTester.IsOutputRedirected())
                return new RedirectedConsole();
            return new DefaultConsole();
        }

        public int BufferWidth { get { return _consoleOutInterface.BufferWidth; } }
        public int WindowWidth { get { return _consoleOutInterface.WindowWidth; } }

        /// <summary>
        /// Output a formatted string at the current cursor position, and move to the beginning of the next line.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        public void WriteLine(string format, params object[] arg)
        {
            Write(format + Environment.NewLine, arg);
        }

        /// <summary>
        /// Output a formatted string at the current cursor position.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        public void Write(string format, params object[] arg)
        {
            var formatted = string.Format(format, arg);
            var components = ColourControlSplitter.Split(formatted);
            _writer.Write(components);
        }

        /// <summary>
        /// Render a renderable object to the console.
        /// </summary>
        /// <param name="renderableData">The object to render.</param>
        public void Write(IConsoleRenderer renderableData)
        {
            RenderData(renderableData, false);
        }

        /// <summary>
        /// Render a renderable object to the console, add a newline.
        /// </summary>
        /// <param name="renderableData">The object to render.</param>
        public void WriteLine(IConsoleRenderer renderableData)
        {
            RenderData(renderableData, true);
        }

        private void RenderData(IConsoleRenderer renderableData, bool endWithNewLine)
        {
            if (_consoleOutInterface.CursorLeft > 0)
                WriteLine();

            int wrappedLines;
            var lines = renderableData.Render(WindowWidth, out wrappedLines).ToList();
            if (!lines.Any()) return;

            foreach (var line in lines.Where((l, i) => i < lines.Count - 1))
            {
                WriteLine(line);
            }
            if (endWithNewLine)
                WriteLine(lines.Last());
            else
                Write(lines.Last());
        }

        public void WrapLine(string format, params object[] arg)
        {
            WriteWrapped(true, format, arg);
        }

        public void Wrap(string format, params object[] arg)
        {
            WriteWrapped(false, format, arg);
        }

        private void WriteWrapped(bool lastLineIsWriteLine, string format, object[] arg)
        {
            var formatted = string.Format(format, arg);
            var lines = ColumnWrapper.WrapValue(formatted, DefaultFormat, _consoleOutInterface.WindowWidth,
                firstLineHangingIndent: _consoleOutInterface.CursorLeft + 1);
            if (lines.Length == 0) return;

            for (var n = 0; n < lines.Length - 1; ++n)
            {
                WriteLine(lines[n]);
            }

            var lastLine = lines.Last();
            if (lastLineIsWriteLine)
                WriteLine(lastLine);
            else
                Write(lastLine);
        }

        public void FormatTable<T>(IEnumerable<T> items, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnSeperator = null)
        {
            var tabular = TabularReport.Format(items, null, _consoleOutInterface.WindowWidth, options: options, columnDivider: columnSeperator);
            foreach (var line in tabular)
                Write(line);
        }

        /// <summary>
        /// Output a new line.
        /// </summary>
        public void WriteLine()
        {
            _writer.NewLine();
        }

        /// <summary>
        /// Read a string from the console.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            return _consoleInInterface.ReadLine();
        }

        public T ReadInput<T>(T template) where T : class
        {
            var impl = new ReadInputImpl<T>(_consoleInInterface, this, template);
            return impl.Result;
        }

        public T ReadInput<T>() where T : class
        {
            var impl = new ReadInputImpl<T>(_consoleInInterface, this);
            return impl.Result;
        }

        public Encoding GetEncoding()
        {
            return _writer.Encoding;
        }
    }
}