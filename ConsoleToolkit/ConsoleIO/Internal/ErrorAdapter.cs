namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// This class wraps the actual stderror stream. Do not use <see href="Console.Error"/> directly, output text via the adapter instead.
    /// This allows the error output to be captured in a unit test without requiring code changes.
    /// </summary>
    internal sealed class ErrorAdapter : ConsoleOperationsImpl, IErrorAdapter
    {
        public ErrorAdapter(IConsoleInterface consoleInterface) : base(consoleInterface)
        {
            
        }
    }
}