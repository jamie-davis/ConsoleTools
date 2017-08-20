using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// Keyword based command inpired by AZ.
    /// </summary>
    [Command("alert")]
    [Keyword("monitor")]
    [Description("Commands to manage metric-based alert rules.")]
    class MonitorAlertCommand
    {
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine(GetType().Name);
        }
    }
}