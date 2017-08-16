using System.Collections.Generic;

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
        /// <param name="options">Options that effect the way in which the table is formatted.</param>
        /// <param name="columnSeperator">The text to use to seperate columns.</param>
        void FormatTable<T>(IEnumerable<T> items, ReportFormattingOptions options = ReportFormattingOptions.Default,
            string columnSeperator = null);

        /// <summary>
        /// Format an enumerable set of rows as a tabular report, using a report definition.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="report">The report definition.</param>
        void FormatTable<T>(Report<T> report);

        /// <summary>
        /// Output a new line.
        /// </summary>
        void WriteLine();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IConsoleInputOperations
    {
        /// <summary>
        /// Read a string from the console.
        /// </summary>
        string ReadLine();

        /// <summary>
        /// Read data from the input stream. <para/>
        /// Supply a type instance as a template that should be filled based on data read from the input stream. 
        /// This overload allows the use of an anonymous type to receive the user input.
        /// For example:<para/>
        /// <code>
        /// var inputData = console.ReadInput(new 
        /// { 
        ///     SomeInt = 0, 
        ///     SomeString = string.Empty 
        /// });
        /// </code>
        /// Would read two lines from the input stream. The first line would be converted to an integer, 
        /// and the second would be converted to a string. An instance of the anonymous type would be returned,
        /// initialised with the values returned from the input stream.
        /// <para/>
        /// <code>
        /// var inputData = console.ReadInput(new Tuple&lt;int, string&gt;());
        /// </code>
        /// This would perform the same operations but the result will be stored in a Tuple instead.
        /// <para/>
        /// 
        /// <code>
        /// var inputData = console.ReadInput(new MyType());
        /// </code>
        /// The type MyType will be examined for public members, which will be read from the input stream.
        /// </summary>
        /// <typeparam name="T">The type to be populated.</typeparam>
        /// <param name="template">An instance of the type. Supplying an instance facilitates use of anonymous types.</param>
        /// <returns>A new instance of T, or null.</returns>
        T ReadInput<T>(T template) where T : class;

        /// <summary>
        /// Read data from the input stream. <para/>
        /// Return an instance of the generic type supplied will be filled with data read from the input stream. 
        /// For example:<para/>
        /// <code>
        /// var inputData = console.ReadInput&lt;Tuple&lt;int, string&gt;&gt;());
        /// </code>
        /// Would read two lines from the input stream. The first line would be converted to an integer, 
        /// and the second would be converted to a string. An instance of the tuple type would be returned,
        /// initialised with the values returned from the input stream.
        /// <para/>
        /// <code>
        /// var inputData = console.ReadInput&lt;MyType&gt;();
        /// </code>
        /// The type MyType will be examined for public members, which will be read from the input stream.
        /// An instance of MyType will be returned.
        /// </summary>
        /// <typeparam name="T">The type to be populated.</typeparam>
        /// <returns>A populated instance of T, or null.</returns>
        T ReadInput<T>() where T : class;

        /// <summary>
        /// Get confirmation from the user. The user will be prompted to input a string value confirming
        /// or not a piece of text supplied in the call.<para/>
        /// The user will be prompted to choose between "true" text or "false" text. The actual text values
        /// are specified globally and the defaults can be overridden.
        /// </summary>
        /// <param name="prompt">The message to display to the user.</param>
        /// <returns>True if the user confirms the message, otherwise false.</returns>
        bool Confirm(string prompt);
    }

    /// <summary>
    /// The interface of a console adapter.
    /// </summary>
    public interface IConsoleAdapter : IConsoleOperations, IConsoleInputOperations
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
    /// The interface of an error adapter.
    /// </summary>
    public interface IErrorAdapter : IConsoleOperations
    {
        /// <summary>
        /// The width of the error buffer in characters.
        /// </summary>
        int BufferWidth { get; }

        /// <summary>
        /// The width of the error window in characters.
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
        int CountWordWrapLineBreaks(int width);
    }
}