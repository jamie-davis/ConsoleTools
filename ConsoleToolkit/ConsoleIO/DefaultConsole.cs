using System;
using System.Text;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This implementation of the console interface is the default version, and interfaces with the actual console.
    /// 
    /// It is a very simple wrapper around the system <see cref="Console"/> and by its nature has no testable methods.
    /// </summary>
    public class DefaultConsole : IConsoleInterface
    {
        /// <summary>
        /// The console colour as it was at construction time. This will be sued to put the console back the way it started when
        /// this class is finalized.
        /// </summary>
        private readonly ConsoleColor _constructionForeground;

        /// <summary>
        /// The console colour as it was at construction time. This will be sued to put the console back the way it started when
        /// this class is finalized.
        /// </summary>
        private readonly ConsoleColor _constructionBackground;

        public DefaultConsole()
        {
            _constructionForeground = Console.ForegroundColor;
            _constructionBackground = Console.BackgroundColor;
        }

        /// <summary>
        /// The current console foreground colour.
        /// </summary>
        public ConsoleColor Foreground
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        /// <summary>
        /// The current console background colour.
        /// </summary>
        public ConsoleColor Background
        {
            get { return Console.BackgroundColor; }
            set { Console.BackgroundColor = value; }
        }

        public int WindowWidth { get { return Console.WindowWidth; } }
        public int BufferWidth { get { return Console.BufferWidth; } }

        public void Write(string data)
        {
            Console.Write(data);
        }

        public void NewLine()
        {
            Console.WriteLine();
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
    }
}