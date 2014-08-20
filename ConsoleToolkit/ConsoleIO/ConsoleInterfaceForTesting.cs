using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class implements the <see cref="IConsoleOutInterface"/> and captures the console output in a format that facilitates
    /// examination of console output in a unit test.
    /// </summary>
    public class ConsoleInterfaceForTesting : IConsoleInterface
    {
        private readonly List<string> _buffer = new List<string>();
        private readonly List<string> _foregroundColourMap = new List<string>();
        private readonly List<string> _backgroundColourMap = new List<string>();

        /// <summary>
        /// The current cursor position.
        /// </summary>
        private int _cursorTop;

        /// <summary>
        /// The current cursor position.
        /// </summary>
        private int _cursorLeft;

        /// <summary>
        /// The curent foreground colour.
        /// </summary>
        private ConsoleColor _foreground;

        /// <summary>
        /// The current background colour.
        /// </summary>
        private ConsoleColor _background;

        /// <summary>
        /// The console encoding.
        /// </summary>
        private Encoding _encoding;

        /// <summary>
        /// The colour code for the current foreground colour.
        /// </summary>
        private char _fgCode;

        /// <summary>
        /// The colour code for the current background colour.
        /// </summary>
        private char _bgCode;

        /// <summary>
        /// The original background colour to which all lines should be initialised.
        /// </summary>
        private char _initialBg;

        /// <summary>
        /// The original foreground colour to which all lines should be initialised.
        /// </summary>
        private char _initialFg;


        /// <summary>
        /// The input stream supplying input for the console.
        /// </summary>
        private TextReader _inputStream;

        /// <summary>
        /// The current foreground colour. This property keeps <see cref="_fgCode"/> aligned with the actual console colour.
        /// </summary>
        public ConsoleColor Foreground
        {
            get { return _foreground; }
            set
            {
                _foreground = value;
                _fgCode = ColourConverter.Convert(_foreground);
            }
        }


        /// <summary>
        /// The current background colour. This property keeps <see cref="_bgCode"/> aligned with the actual console colour.
        /// </summary>
        public ConsoleColor Background
        {
            get { return _background; }
            set
            {
                _background = value; 
                _bgCode = ColourConverter.Convert(_background);

            }
        }

        /// <summary>
        /// The width of the window. This is the visible part of the display. It is possible for this to be less than the width of the buffer.
        /// </summary>
        public int WindowWidth { get; set; }

        /// <summary>
        /// The width of the buffer. This is the width of the data in the console window and can be wider than the actual window itself.
        /// </summary>
        public int BufferWidth { get; set; }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        public int CursorLeft
        {
            get { return _cursorLeft; }
            set { _cursorLeft = value; }
        }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        public int CursorTop
        {
            get { return _cursorTop; }
            set { _cursorTop = value; }
        }

        public Encoding Encoding { get { return _encoding; } }

        /// <summary>
        /// The constructor sets default values for various console properties and allows an encoding to be specified.
        /// </summary>
        public ConsoleInterfaceForTesting(Encoding encoding = null)
        {
            _encoding = encoding ?? Encoding.Default;
            Foreground = ConsoleColor.DarkGray;
            Background = ConsoleColor.Black;
            _initialBg = _bgCode;
            _initialFg = _fgCode;
            WindowWidth = 40;
            BufferWidth = 60;

            CreateBufferTo(0); //ensure that the buffer contains the first line.
        }

        /// <summary>
        /// Write some text to the console buffer. Does not add a line feed.
        /// </summary>
        /// <param name="data">The text data to write. This must not contain colour instructions.</param>
        public void Write(string data)
        {
            while (data.Contains(Environment.NewLine) || (data.Length + _cursorLeft > BufferWidth))
            {
                string nextData;
                var usableLength = BufferWidth - _cursorLeft;
                var newlinePos = data.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                if (newlinePos >= 0 && newlinePos < usableLength)
                {
                    nextData = data.Substring(0, newlinePos);
                    data = data.Substring(newlinePos + Environment.NewLine.Length);
                    Write(nextData);
                    NewLine();
                }
                else
                {
                    nextData = data.Substring(0, usableLength);
                    data = data.Substring(usableLength);
                    Write(nextData);
                }

            }

            CreateBufferTo(_cursorTop);

            var fgColorData = new string(_fgCode, data.Length);
            var bgColorData = new string(_bgCode, data.Length);

            OverWrite(_buffer, _cursorTop, _cursorLeft, data);
            OverWrite(_foregroundColourMap, _cursorTop, _cursorLeft, fgColorData);
            OverWrite(_backgroundColourMap, _cursorTop, _cursorLeft, bgColorData);

            _cursorLeft += data.Length;

            if (_cursorLeft >= BufferWidth)
            {
                _cursorTop++;
                _cursorLeft = 0;
                CreateBufferTo(_cursorTop);
            }

        }

        /// <summary>
        /// Overlay some text in an existing buffer. The method will discard any data that would overflow the buffer width.
        /// </summary>
        /// <param name="buffer">The buffer line array.</param>
        /// <param name="lineIndex">The index of the line to overwrite</param>
        /// <param name="overwritePosition">The position within the line to overwrite.</param>
        /// <param name="data">The text to place in the buffer at the specified position.</param>
        private void OverWrite(IList<string> buffer, int lineIndex, int overwritePosition, string data)
        {
            var line = buffer[lineIndex];

            if (overwritePosition >= line.Length)
                return;

            var newLine = overwritePosition > 0 ? line.Substring(0, overwritePosition) : string.Empty;
            newLine += data;

            if (newLine.Length < line.Length)
                newLine += line.Substring(newLine.Length); //copy the remainder of the line from the original data

            if (newLine.Length > BufferWidth)
                newLine = newLine.Substring(0, BufferWidth);
            else if (newLine.Length < BufferWidth)
                newLine += new string(' ', BufferWidth - newLine.Length);

            buffer[lineIndex] = newLine;
        }

        /// <summary>
        /// Ensure that the buffer contains the specified line.
        /// </summary>
        /// <param name="ix">The zero based index of the line that must exist.</param>
        private void CreateBufferTo(int ix)
        {
// ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (ix >= _buffer.Count)
            {
                _buffer.Add(new string(' ', BufferWidth));
                _foregroundColourMap.Add(new string(_initialFg, BufferWidth));
                _backgroundColourMap.Add(new string(_initialBg, BufferWidth));
            }
        }

        /// <summary>
        /// Write a newline to the buffer.
        /// </summary>
        public void NewLine()
        {
            _cursorTop++;
            _cursorLeft = 0;
            CreateBufferTo(_cursorTop);
        }

        /// <summary>
        /// Return the entire buffer for testing purposes. It is possible to get just the text, just the colour information or all of the data.
        /// </summary>
        /// <param name="format">Enumeration value that specifies what should be returned.</param>
        /// <returns>A large string containing the requested data.</returns>
        public string GetBuffer(ConsoleBufferFormat format = ConsoleBufferFormat.TextOnly)
        {
            if (format == ConsoleBufferFormat.TextOnly)
                return string.Join(Environment.NewLine, _buffer);

            var allLines = _buffer.Where(b => format != ConsoleBufferFormat.ColourOnly)
                .Select((b, i) => new {Key = string.Format("{0:0000}C", i), Text = "+" + b})
                .Concat(_foregroundColourMap.Select((b, i) => new {Key = string.Format("{0:0000}A", i), Text = "F" + b}))
                .Concat(_backgroundColourMap.Select((b, i) => new {Key = string.Format("{0:0000}B", i), Text = "B" + b}))
                .OrderBy(l => l.Key)
                .Select(l => l.Text);

            return string.Join(Environment.NewLine, allLines);
        }

        /// <summary>
        /// Indicate whether console input is redirected or not. This will effect the handling of invalid
        /// input on the stream.
        /// </summary>
        public bool InputIsRedirected { get; set; }

        /// <summary>
        /// Read a line of text from the console. The data for this operation is provided using the <see cref="SetInputStream"/> method.
        /// </summary>
        /// <returns>The next line of text.</returns>
        public string ReadLine()
        {
            CheckInputStream();
            var readLine = _inputStream.ReadLine();
            if (readLine != null)
            {
                Write(readLine);
                NewLine();
            }
            return readLine;
        }

        private void CheckInputStream()
        {
            if (_inputStream == null)
                throw new NoInputStreamSet();
        }

        /// <summary>
        /// Provide a text stream to provide the data for the console input stream.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        public void SetInputStream(TextReader stream)
        {
            _inputStream = stream;
        }

        private class NoInputStreamSet : Exception
        {
        }
    }
}
