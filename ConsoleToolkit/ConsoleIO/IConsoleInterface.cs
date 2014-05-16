using System;
using Microsoft.SqlServer.Server;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This interface describes a class that interacts with the system console i.e. the <see cref="Console"/> in <c>Console.WriteLine(...</c> calls.
    /// 
    /// Implement this interface to intercept console calls in order to redirect the input and output, for example, in a unit test.
    /// </summary>
    public interface IConsoleInterface
    {
        /// <summary>
        /// Test to see if console output is redirected.
        /// 
        /// The test only determines whether StdOut is redirected. StdErr redirection cannot be detected without unsafe code.
        /// </summary>
        /// <returns>True if console output is redirected, otherwise false.</returns>
        bool IsOutputRedirected();

        /// <summary>
        /// Output a composite formatted string to the console, and move the cursor to a new line.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg">An array of objects to write using <see cref="format"/>.</param>
        void WriteLine(string format, params object[] arg);
    }
}