using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace ConsoleToolkitDemo
{
    [Command("help")]
    [Description("Display help text.")]
    class HelpCommand
    {
        [Positional(0, DefaultValue = null)]
        public string Topic { get; set; }
    }
}