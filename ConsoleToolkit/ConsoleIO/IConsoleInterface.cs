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
        /// The current foreground colour for console output.
        /// </summary>
        ConsoleColor Foreground { get; set; }

        /// <summary>
        /// The current background colour for console output.
        /// </summary>
        ConsoleColor Background { get; set; }

        /// <summary>
        /// The width of the console window - i.e. the visible part of the buffer.
        /// <seealso cref="BufferWidth"/>
        /// </summary>
        int WindowWidth { get; }

        /// <summary>
        /// The width of the console buffer. This may be greater than the window width.
        /// <seealso cref="WindowWidth"/>
        /// </summary>
        int BufferWidth { get; }

        /// <summary>
        /// Test to see if console output is redirected.
        /// 
        /// The test only determines whether StdOut is redirected. StdErr redirection cannot be detected without unsafe code.
        /// </summary>
        /// <returns>True if console output is redirected, otherwise false.</returns>
        bool IsOutputRedirected();

        /// <summary>
        /// Output a string to the console in the current cursor position.
        /// </summary>
        /// <param name="data">The text to output.</param>
        void Write(string data);

        /// <summary>
        /// Output a new line to the console at the current cursor position. The cursor will move the beginning of the next line.
        /// </summary>
        void NewLine();

        /// <summary>
        /// The current cursor position.
        /// </summary>
        int CursorLeft { get; set; }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        int CursorTop { get; set; }

    }
}