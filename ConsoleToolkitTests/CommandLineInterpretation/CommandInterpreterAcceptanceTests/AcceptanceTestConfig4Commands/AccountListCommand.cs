using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// Keyword based command inpired by AZ.
    /// </summary>
    [Command("list")]
    [Keyword("account")]
    [Description("Get a list of subscriptions for the account.")]
    class AccountListCommand
    {
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine(GetType().Name);
        }
    }
}