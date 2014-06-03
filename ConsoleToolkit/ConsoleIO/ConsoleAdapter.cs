using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using ConsoleToolkit.ConsoleIO.Internal;
using Microsoft.SqlServer.Server;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class wraps the actual console. Do not use the console directly, output text via the adapter instead. 
    /// This allows the console output to be captured in a unit test without requiring code changes.
    /// </summary>
    public class ConsoleAdapter : IConsoleAdapter
    {
        private readonly IConsoleInterface _consoleInterface;
        private readonly ColourWriter _writer;

        public ConsoleAdapter(IConsoleInterface consoleInterface = null)
        {
            _consoleInterface = consoleInterface ?? new DefaultConsole();
            _writer = new ColourWriter(_consoleInterface);
        }

        public int BufferWidth { get { return _consoleInterface.BufferWidth; } }
        public int WindowWidth { get { return _consoleInterface.WindowWidth; } }

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

        public void FormatTable<T>(IEnumerable<T> items)
        {
            var tabular = TabularReport.Format(items, null, _consoleInterface.WindowWidth);
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