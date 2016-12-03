namespace ConsoleToolkit.ApplicationStyles.Internals
{
    /// <summary>
    /// This enumeration describes the type of scan that the <see cref="CommandAssemblyScanner"/> can perform.
    /// </summary>
    public enum CommandScanType
    {
        /// <summary>
        /// Return only commands that can be run in an interactive session.
        /// </summary>
        InteractiveCommands,

        /// <summary>
        /// Return only commands that can be run in a command line only execution.
        /// </summary>
        NonInteractiveCommands,

        /// <summary>
        /// Return all commands, regardless of their validity.
        /// </summary>
        AllCommands
    }
}