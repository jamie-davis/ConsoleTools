using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace InteractiveCommandSample.Commands
{
    [Command]
    [Description("Display command help text")]
    public class HelpCommand
    {
        [Positional]
        [Description("The command on which help is required.")]
        public string Topic { get; set; }
    }
}