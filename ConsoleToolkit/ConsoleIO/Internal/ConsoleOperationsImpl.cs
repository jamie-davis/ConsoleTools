using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class ConsoleOperationsImpl : IConsoleOperations
    {
        private static readonly ColumnFormat DefaultFormat = new ColumnFormat();

        private readonly IConsoleOutInterface _consoleOutInterface;
        private readonly ColourWriter _writer;
        private int _prefixLength;

        internal ConsoleOperationsImpl(IConsoleOutInterface consoleOutInterface, string prefixText = null)
        {
            _consoleOutInterface = consoleOutInterface;
            _writer = new ColourWriter(_consoleOutInterface);
            if (prefixText != null)
            {
                _writer.PrefixText = prefixText;
                _prefixLength = prefixText.Length;
            }
            else
                _prefixLength = 0;
        }

        public int BufferWidth { get { return _consoleOutInterface.BufferWidth - _prefixLength; } }
        public int WindowWidth { get { return _consoleOutInterface.WindowWidth - _prefixLength; } }

        protected Encoding Encoding { get { return _writer.Encoding; } }

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
            var anyArgs = !(arg == null || arg.Length == 0);

            var formatted = anyArgs ? string.Format(format, arg) : format;
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
            var formatted = FormatData(format, arg);
            var allowanceForCursorPosition = _consoleOutInterface.CursorLeft > 0 
                ? _consoleOutInterface.CursorLeft - _prefixLength //discount the prefix as this will not be counted in the window width
                : 0; //no data on the line, so no allowance required.
            var lines = ColumnWrapper.WrapValue(formatted, DefaultFormat, WindowWidth,
                                                firstLineHangingIndent: allowanceForCursorPosition);
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

        private static string FormatData(string format, object[] arg)
        {
            if (arg.Length == 0) return format;

            return string.Format(format, arg);
        }

        public void FormatTable<T>(IEnumerable<T> items, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnSeperator = null)
        {
            var tabular = TabularReport.Format<T, T>(items, null, WindowWidth, options: options, columnDivider: columnSeperator);
            foreach (var line in tabular)
                Write(line);
        }

        public void FormatTable<T>(Report<T> report)
        {
            var tabular = ReportExecutor.GetLines(report, WindowWidth);
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
    }
}