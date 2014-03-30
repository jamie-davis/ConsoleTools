using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class implements an adapter suitable for formatting text for output to a console.
    /// </summary>
    public class ConsoleAdapter : IConsole
    {
        private TextWriter _console;
        private TextWriter _error;
        private int _consoleWidth;

        /// <summary>
        /// Attach the receivers for the console output.
        /// </summary>
        /// <param name="console">The writer to receive normal output.</param>
        /// <param name="error">The writer to receive error output.</param>
        /// <param name="consoleWidth">The display width to wrap on.</param>
        public ConsoleAdapter(TextWriter console, TextWriter error, int consoleWidth)
        {
            _console = console;
            _error = error;
            _consoleWidth = consoleWidth;
        }

        public void DisplayColumns<T>(IEnumerable<T> input, bool showHeadings = true)
        {
            DisplayPopertiesInColumns(input, _console, showHeadings);
        }

        public void DisplayColumnsOnError<T>(IEnumerable<T> input, bool showHeadings = true)
        {
            DisplayPopertiesInColumns(input, _error, showHeadings);
        }

        private void DisplayPopertiesInColumns<T>(IEnumerable<T> input, TextWriter console, bool showHeadings = true)
        {
            var data = input.ToList();
            var reportingType = typeof (T);
            var props = reportingType.GetProperties();
            var reportMaxWidths = props
                .Select(p =>
                {
                    var width = Math.Max(showHeadings ? p.Name.Length : 0, data.Max(d => p.GetValue(d).ToString().Length))*
                                (IsNumeric(p.PropertyType) ? 1 : -1);

                    return new
                    {
                        Prop = p,
                        Width = Math.Abs(width),
                        ItemFormatter = new Func<T, string>(r => string.Format("{0," + width + "}", p.GetValue(r))),
                        HeadingText =
                            string.Format("{0,-" + Math.Abs(width) + "}" + Environment.NewLine + "{1}", p.Name,
                                new string('-', Math.Abs(width)))
                    };
                })
                .ToList();

            var headings = Enumerable.Repeat(reportMaxWidths.Select(p => new {Prop = p, Text = p.HeadingText}).ToList(), 1);

            var rows = input
                .Select(r => reportMaxWidths
                    .Select(p => new {Prop = p, Text = p.ItemFormatter(r)})
                    .ToList())
                .ToList();

            foreach (var row in  showHeadings ? headings.Concat(rows) : rows)
            {
                var line = string.Empty;
                var widthUsed = 0;
                foreach (var col in row)
                {
                    line = TextFormatter.MergeBlocks(line, widthUsed > 0 ? ++widthUsed : 0, col.Text);
                    widthUsed += col.Prop.Width;
                }
                console.Write(line);
            }
        }

        private bool IsNumeric(Type propertyType)
        {
            var numericTypeCodes = new List<TypeCode>
            {
                TypeCode.Int16,
                TypeCode.Int32,
                TypeCode.Int64,
                TypeCode.UInt16,
                TypeCode.UInt32,
                TypeCode.UInt64,
                TypeCode.Double,
                TypeCode.Single,
                TypeCode.Decimal,
            }; 
            return numericTypeCodes.Contains(Type.GetTypeCode(propertyType));
        }

        public void PrintLine(string data, params object[] args)
        {
            _console.Write(FormatLine(data, args));
        }

        public void WrapError(string data, params object[] args)
        {
            _error.Write(Format(data, args));
        }

        public void Error(string data, params object[] args)
        {
            _error.Write(data, args);
        }

        public void ErrorLine(string data, params object[] args)
        {
            _error.Write(FormatLine(data, args));
        }

        private string FormatLine(string data, object[] args)
        {
            if (data == null)
                return Environment.NewLine;
            return TextFormatter.FormatBlock(_consoleWidth, args == null || args.Length == 0 ? data : string.Format(data + Environment.NewLine, args));
        }

        public int Width
        {
            get { return _consoleWidth; }
        }

        public void WrapOutput(string data, params object[] args)
        {
            _console.Write(Format(data, args));
        }

        public void Print(string data, params object[] args)
        {
            _console.Write(data, args);
        }

        private string Format(string data, object[] args)
        {
            return TextFormatter.FormatBlock(_consoleWidth, string.Format(data, args));
        }
    }
}
