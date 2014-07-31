using System.Collections.Generic;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// The base set of operations supported for writing data to the console.
    /// </summary>
    public interface IConsoleOperations
    {

        /// <summary>
        /// Output a formatted string at the current cursor position, and move to the beginning of the next line.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        void WriteLine(string format, params object[] arg);

        /// <summary>
        /// Output a formatted string at the current cursor position.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        void Write(string format, params object[] arg);

        /// <summary>
        /// Output a formatted string at the current cursor position, using word wrapping. Then move to the beginning of the next line.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        void WrapLine(string format, params object[] arg);

        /// <summary>
        /// Output a formatted string at the current cursor position, but use word wrapping.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        void Wrap(string format, params object[] arg);

        /// <summary>
        /// Render a renderable object to the console.
        /// </summary>
        /// <param name="renderableData">The object to render.</param>
        void Write(IConsoleRenderer renderableData);

        /// <summary>
        /// Render a renderable object to the console, add a newline.
        /// </summary>
        /// <param name="renderableData">The object to render.</param>
        void WriteLine(IConsoleRenderer renderableData);

        /// <summary>
        /// Format an enumerable set of rows as a tabular report.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items to be formatted.</param>
        void FormatTable<T>(IEnumerable<T> items, ReportFormattingOptions options = ReportFormattingOptions.None);

        /// <summary>
        /// Output a new line.
        /// </summary>
        void WriteLine();
    }

    /// <summary>
    /// The interface of a console adapter.
    /// </summary>
    public interface IConsoleAdapter : IConsoleOperations
    {
        /// <summary>
        /// The width of the console buffer in characters.
        /// </summary>
        int BufferWidth { get; }

        /// <summary>
        /// The width of the console window in characters.
        /// </summary>
        int WindowWidth { get; }

    }

    /// <summary>
    /// This interface defines the functionality required to support rendering recorded console activity
    /// in the various ways that a the data is used by the toolkit.
    /// </summary>
    public interface IConsoleRenderer
    {
        IEnumerable<string> Render(int width, out int wrappedLines);
        int GetFirstWordLength(int tabLength);
        int GetLongestWordLength(int tabLength);
        int CountWordWrapLineBreaks(ColumnFormat format, int width);
    }
}