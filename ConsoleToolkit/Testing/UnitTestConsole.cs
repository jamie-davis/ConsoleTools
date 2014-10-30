using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.Testing
{
    /// <summary>
    /// This class instantiates a console and an error adapter for use in testing command implementations. Both adapters write to the same
    /// console interface.
    /// <see cref="ConsoleInterfaceForTesting"/> 
    /// </summary>
    public class UnitTestConsole
    {
        private readonly ConsoleInterfaceForTesting _consoleInterface;
        private readonly ConsoleAdapter _console;
        private readonly ErrorAdapter _error;

        public UnitTestConsole(string programName = null)
        {
            _consoleInterface = new ConsoleInterfaceForTesting();
            var errorPrefix = string.Format("{0}: ", programName ?? "error");

            _console = new ConsoleAdapter(_consoleInterface);
            _error = new ErrorAdapter(_consoleInterface, errorPrefix);
        }

        /// <summary>
        /// The console adapter.
        /// </summary>
        public IConsoleAdapter Console { get { return _console; } }

        /// <summary>
        /// The error adapter.
        /// </summary>
        public IErrorAdapter Error { get { return _error; } }

        /// <summary>
        /// The test console interface that receives the output written to the adapters. Interrogate this to determine what output was written.
        /// </summary>
        public ConsoleInterfaceForTesting Interface { get { return _consoleInterface; } }
    }
}
