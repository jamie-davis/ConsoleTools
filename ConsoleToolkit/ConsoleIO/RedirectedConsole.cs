using System;
using System.IO;
using System.Text;

namespace ConsoleToolkit.ConsoleIO
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

        public RedirectedConsole(ConsoleStream stream)
        {
            try
            {
                //this will throw if there is no real console.
                _width = Console.BufferWidth;
                Encoding = Console.OutputEncoding;
            }
            catch (Exception)
            {
                _width = DefaultWidth;
                Encoding = Encoding.Default;
            }

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
            var charactersLeft = BufferWidth - CursorLeft;
            while (data.Length >= charactersLeft)
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