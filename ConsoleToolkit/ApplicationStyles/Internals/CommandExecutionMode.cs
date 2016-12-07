namespace ConsoleToolkit.ApplicationStyles.Internals
{
    /// <summary>
    /// The mode in which command execution is taking place.
    /// </summary>
    public enum CommandExecutionMode
    {
        /// <summary>
        /// Normal, based on command line parameters
        /// </summary>
        CommandLine,

        /// <summary>
        /// Inside an interactive session
        /// </summary>
        Interactive
    }
}