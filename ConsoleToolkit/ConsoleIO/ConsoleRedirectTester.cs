using System;

namespace ConsoleToolkit.ConsoleIO
{
    internal class ConsoleRedirectTester
    {
        private bool _consoleRedirectionTested;
        private bool _consoleRedirected;

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

    }
}