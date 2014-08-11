using System;
using System.Text;

namespace ConsoleToolkit.ConsoleIO
{
    public class RedirectedConsole : IConsoleInterface
    {
        private int _width;
        private int _cursorLeft;
        private int _cursorTop;
        private const int DefaultWidth = 133;

        public RedirectedConsole()
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
                Console.WriteLine(data.Substring(0, charactersLeft));
                data = data.Substring(charactersLeft);
                charactersLeft = BufferWidth;
                _cursorLeft = 0;
                _cursorTop++;
            }

            Console.Write(data);
            _cursorLeft += data.Length;
        }

        public void NewLine()
        {
            Console.WriteLine();
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