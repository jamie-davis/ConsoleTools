using System;
using System.IO;
using System.Text;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This implementation of the console interface is the default version, and interfaces with the actual console.
    /// 
    /// It is a very simple wrapper around the system <see cref="Console"/> and by its nature has no testable methods.
    /// </summary>
    internal class DefaultConsole : IConsoleInterface
    {
        /// <summary>
        /// The console colour as it was at construction time. This will be used to put the console back the way it started when
        /// this class is finalized.
        /// </summary>
        private readonly ConsoleColor _constructionForeground;

        /// <summary>
        /// The console colour as it was at construction time. This will be used to put the console back the way it started when
        /// this class is finalized.
        /// </summary>
        private readonly ConsoleColor _constructionBackground;

        /// <summary>
        /// The console colour that was last set. This will be used to set the colour of the actual console just before output
        /// is displayed.
        /// </summary>
        private ConsoleColor _currentForeground;

        /// <summary>
        /// The console colour that was last set. This will be used to set the colour of the actual console just before output
        /// is displayed.
        /// </summary>
        private ConsoleColor _currentBackground;

        /// <summary>
        /// The console stream to receive the output.
        /// </summary>
        private TextWriter _stream;

        public DefaultConsole(ConsoleStream stream)
        {
            _constructionForeground = Console.ForegroundColor;
            _constructionBackground = Console.BackgroundColor;

            _currentForeground = _constructionForeground;
            _currentBackground = _constructionBackground;

            _stream = stream == ConsoleStream.Out ? Console.Out : Console.Error;
        }

        /// <summary>
        /// The current console foreground colour.
        /// </summary>
        public ConsoleColor Foreground
        {
            get { return _currentForeground; }
            set { _currentForeground = value; }
        }

        /// <summary>
        /// The current console background colour.
        /// </summary>
        public ConsoleColor Background
        {
            get { return _currentBackground; }
            set { _currentBackground = value; }
        }

        public int WindowWidth { get { return Console.WindowWidth; } }
        public int BufferWidth { get { return Console.BufferWidth; } }

        public void Write(string data)
        {
            ResetConsoleColours();
            _stream.Write(data);
        }

        public void NewLine()
        {
            ResetConsoleColours();
            _stream.WriteLine();
        }

        public int CursorLeft 
        {
            get { return Console.CursorLeft; }
            set { Console.CursorLeft = value; } 
        }

        public int CursorTop
        {
            get { return Console.CursorTop; }
            set { Console.CursorTop = value; }
        }

        public Encoding Encoding
        {
            get { return Console.OutputEncoding; }
        }

        ~DefaultConsole()
        {
            //restore the console's original colour scheme
            Console.ForegroundColor = _constructionForeground;
            Console.BackgroundColor = _constructionBackground;
        }

        public bool InputIsRedirected 
        {
            get
            {
#if TARGET_FRAMEWORK_4
                return ConsoleRedirectionStateDetector.IsInputRedirected;
#else
                return Console.IsInputRedirected;
#endif
            } 
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        private void ResetConsoleColours()
        {
            Console.ForegroundColor = _currentForeground;
            Console.BackgroundColor = _currentBackground;
        }
    }
}