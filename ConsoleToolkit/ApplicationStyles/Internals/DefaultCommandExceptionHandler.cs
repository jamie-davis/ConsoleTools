using System;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal static class DefaultCommandExceptionHandler
    {
        public static void Handler(IConsoleAdapter console, IErrorAdapter error, Exception exception, object command)
        {
            error.WrapLine("Processing halted due to exception:");
            error.WrapLine(exception.Message);
        }
    }


}
