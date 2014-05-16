namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class wraps the actual console. Do not use the console directly, output text via the adapter instead. 
    /// This allows the console output to be captured in a unit test without requiring code changes.
    /// </summary>
    public class ConsoleAdapter : IConsoleAdapter
    {
        private readonly IConsoleInterface _consoleInterface;

        public ConsoleAdapter(IConsoleInterface consoleInterface = null)
        {
            _consoleInterface = consoleInterface ?? new DefaultConsole();
        }
    }
}