namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// This interface defines a mechanism for translating option names for display in help text.
    /// </summary>
    public interface IOptionNameHelpAdorner
    {
        /// <summary>
        /// Return the option name with the appropriate adornment for display in help text.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>The option name with the appropriate adornment.</returns>
        string Adorn(string name);
    }
}