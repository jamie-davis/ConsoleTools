using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// This is the help command.
    /// </summary>
    [Command("help")]
    [Description("Display this help text.")]
    public class HelpCommand
    {
        [Positional(DefaultValue = null)]
        [Description("The command that you require help with. This parameter is optional.")]
        public string Topic { get; set; }
    }
}