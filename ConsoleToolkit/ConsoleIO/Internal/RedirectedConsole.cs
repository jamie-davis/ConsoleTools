using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ConsoleToolkit.Exceptions;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class RedirectedConsole : IConsoleInterface
    {
        private int _width;
        private int _cursorLeft;
        private int _cursorTop;
        private const int DefaultWidth = 133;

        /// <summary>
        /// The console stream to receive the output.
        /// </summary>
        private TextWriter _stream;

        public RedirectedConsole(ConsoleStream stream) : this(stream, null)
        {
            
        }
        
        /// <summary>
        /// Specialised constructor allowing specification of the stream being redirected and am optional default width if the buffer width is not available. 
        /// </summary>
        /// <param name="stream">The stream being redirected, i.e. stdout or stderr</param>
        /// <param name="overrideDefaultWidth">The default width to use when a specific buffer width is not available.</param>
        public RedirectedConsole(ConsoleStream stream, int? overrideDefaultWidth = null)
        {
            var defaultWidth = overrideDefaultWidth ?? DefaultWidth;
            try
            {
                //this may throw if there is no real console.
                _width = Console.BufferWidth;
                Encoding = Console.OutputEncoding;
            }
            catch (Exception)
            {
                _width = defaultWidth;
                Encoding = Encoding.Default;
            }

            if (_width == 0)
                _width = defaultWidth;

            _stream = stream == ConsoleStream.Out ? Console.Out : Console.Error;
        }

        public ConsoleColor Foreground { get; set; }
        public ConsoleColor Background { get; set; }

        public int WindowWidth
        {
            get { return _width; }
        }

        public int BufferWidth
        {
            get { return _width; }
        }

        public void Write(string data)
        {
            Debug.Assert(BufferWidth > 0);
            var charactersLeft = BufferWidth - CursorLeft;
            while (data.Length >= charactersLeft && BufferWidth > 0)
            {
                _stream.WriteLine(data.Substring(0, charactersLeft));
                data = data.Substring(charactersLeft);
                charactersLeft = BufferWidth;
                _cursorLeft = 0;
                _cursorTop++;
            }

            _stream.Write(data);
            _cursorLeft += data.Length;
        }

        public void NewLine()
        {
            _stream.WriteLine();
        }

        public int CursorLeft
        {
            get { return _cursorLeft; }
            set { throw new InvalidOperationForRedirectedConsole(); }
        }

        public int CursorTop
        {
            get { return _cursorTop; }
            set { throw new InvalidOperationForRedirectedConsole(); }
        }

        public Encoding Encoding { get; private set; }

        public bool InputIsRedirected { get{throw new NotImplementedException();} }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }
    }
}