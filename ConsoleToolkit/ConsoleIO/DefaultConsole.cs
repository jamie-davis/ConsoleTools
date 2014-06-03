using System;
using System.Xml.Schema;

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

        private bool _consoleRedirected;
        private bool _consoleRedirectionTested;

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


        /// <summary>
        /// Determine whether the console's output is redirected.
        /// </summary>
        /// <returns></returns>
        public bool IsOutputRedirected()
        {
            if (!_consoleRedirectionTested)
            {
                try
                {
                    //This will throw if the console output is redirected.
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);

                    _consoleRedirected = false;
                }
                catch
                {
                    _consoleRedirected = true;
                }

                _consoleRedirectionTested = true;
            }

            return _consoleRedirected;
        }

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

        ~DefaultConsole()
        {
            //restore the console's original colour scheme
            Console.ForegroundColor = _constructionForeground;
            Console.BackgroundColor = _constructionBackground;
        }
    }
}