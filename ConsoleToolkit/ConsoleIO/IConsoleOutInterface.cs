using System;
using System.Text;
using Microsoft.SqlServer.Server;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This interface describes a class that interacts with the system console from an output viewpoint i.e. the <see cref="Console"/> in <c>Console.WriteLine(...</c> calls.
    /// 
    /// Implement this interface to intercept console calls in order to redirect output, for example, in a unit test.
    /// </summary>
    public interface IConsoleOutInterface
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

        /// <summary>
        /// The encoding being used by the console.
        /// </summary>
        Encoding Encoding { get; }
    }

    /// <summary>
    /// This interface describes a class that interacts with the system console from an input viewpoint i.e. the <see cref="Console"/> in <c>Console.ReadLine()</c> calls.
    /// 
    /// Implement this interface to intercept console calls in order to redirect the input, for example, in a unit test.
    /// </summary>
    public interface IConsoleInInterface
    {
        bool InputIsRedirected { get; }
        string ReadLine();

    }

    /// <summary>
    /// A console interface implementing both input and output.
    /// </summary>
    public interface IConsoleInterface : IConsoleOutInterface, IConsoleInInterface
    {
    }

    /// <summary>
    /// Identifies one of the console streams.
    /// </summary>
    internal enum ConsoleStream
    {
        Out,
        Error
    }
}