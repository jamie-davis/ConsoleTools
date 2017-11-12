using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.GlobalOptionAcceptanceTestCommands
{
    [Keyword("client")]
    [Command("del")]
    [Description("Delete a client from the system.")]
    public class ClientDelCommand
    {
        [Positional]
        [Description("The guid of the client")]
        public string ClientGuid { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
        }
    }
}