namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class implements the <see cref="IConsoleInterface"/> and captures the console output in a format that facilitates
    /// examination of console output in a unit test.
    /// </summary>
    public class ConsoleInterfaceForTesting : IConsoleInterface
    {
        /// <summary>
        /// In a unit test, the full capabilities of the console are simulated.
        /// </summary>
        /// <returns>True if console output is redirected.</returns>
        public bool IsOutputRedirected()
        {
            return false;
        }

        public void WriteLine(string format, params object[] arg)
        {

        }
    }
}