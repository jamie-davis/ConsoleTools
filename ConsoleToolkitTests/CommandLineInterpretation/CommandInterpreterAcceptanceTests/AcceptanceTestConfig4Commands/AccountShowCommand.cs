using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// Keyword based command inpired by AZ.
    /// </summary>
    [Command("show")]
    [Keyword("account")]
    [Description("Show the details of a subscription.")]
    class AccountShowCommand
    {
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine(GetType().Name);
        }
    }
}