using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.GlobalOptionAcceptanceTestCommands
{
    [Keyword("client", "Client operations")]
    [Command("add")]
    [Description("Add a new client with default settings.")]
    public class ClientAddCommand
    {
        [Positional]
        [Description("The name of the client")]
        public string Name { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
        }
    }
}
