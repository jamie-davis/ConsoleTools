using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// Keyword based command inpired by AZ.
    /// </summary>
    [Command("table")]
    [Keyword("storage", "Durable, highly available, and massively scalable cloud storage.")]
    [Description("NoSQL key-value storage using semi-structured datasets.")]
    class StorageTableCommand
    {
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine(GetType().Name);
        }
    }
}