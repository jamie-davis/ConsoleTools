using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// This class wraps the actual console. Do not use the console directly, output text via the adapter instead. 
    /// This allows the console output to be captured in a unit test without requiring code changes.
    /// </summary>
    internal sealed class ConsoleAdapter : ConsoleOperationsImpl, IConsoleAdapter
    {
        private IConsoleInInterface _consoleInInterface;

        public ConsoleAdapter(IConsoleOutInterface consoleOutInterface, IConsoleInInterface consoleInInterface = null) : base(consoleOutInterface)
        {
            _consoleInInterface = consoleInInterface;
        }

        public ConsoleAdapter(IConsoleInterface consoleInterface) : base(consoleInterface)
        {
            _consoleInInterface = consoleInterface;
        }

        /// <summary>
        /// Read a string from the console.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            return _consoleInInterface.ReadLine();
        }

        public T ReadInput<T>(T template) where T : class
        {
            var impl = new ReadInputImpl<T>(_consoleInInterface, this, template);
            return impl.Result;
        }

        public T ReadInput<T>() where T : class
        {
            var impl = new ReadInputImpl<T>(_consoleInInterface, this);
            return impl.Result;
        }

        public bool Confirm(string prompt)
        {
            var info = Toolkit.Options.ConfirmationInfo;
            var confirmed = ReadInput(Read.Int().Prompt(prompt)
                .Option(1, info.YesText, info.YesPrompt)
                .Option(2, info.NoText, info.NoPrompt));
            return confirmed == 1;
        }

        public Encoding GetEncoding()
        {
            return base.Encoding;
        }
    }
}