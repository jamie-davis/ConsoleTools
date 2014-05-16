using System;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This implementation of the console interface is the default version, and interfaces with the actual console.
    /// 
    /// It is a very simple wrapper around the system <see cref="Console"/> and by its nature has no testable methods.
    /// </summary>
    public class DefaultConsole : IConsoleInterface
    {
        public bool IsOutputRedirected()
        {
            try
            {
                //This will throw if the console output is redirected.
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            }
            catch
            {
                return true;
            }

            return false;
        }

        public void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }
    }
}