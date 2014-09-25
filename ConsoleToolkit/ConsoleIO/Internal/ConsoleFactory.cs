namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class ConsoleFactory
    {
        internal IConsoleInterface Console { get { return _console; } }
        internal IConsoleInterface Error { get { return _error; } }

        private IConsoleInterface _console;
        private IConsoleInterface _error;

        public ConsoleFactory(IConsoleRedirectTester tester = null)
        {
            var redirectTester = tester ?? new ConsoleRedirectTester();
            _console = redirectTester.IsOutputRedirected() ? new RedirectedConsole(ConsoleStream.Out) : new DefaultConsole(ConsoleStream.Out) as IConsoleInterface;
            _error = redirectTester.IsErrorRedirected() ? new RedirectedConsole(ConsoleStream.Error) : new DefaultConsole(ConsoleStream.Error) as IConsoleInterface;
        }
    }
}
