using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// Keyword based command inpired by AZ.
    /// </summary>
    [Command("log-profiles")]
    [Keyword("monitor", "Commands to manage Azure Monitor service.")]
    [Description("Commands to manage the log profiles assigned to Azure subscription.")]
    class MonitorLogProfilesCommand
    {
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine(GetType().Name);
        }
    }
}