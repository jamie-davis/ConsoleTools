using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.GlobalOptionAcceptanceTestCommands
{
    [GlobalOptions]
    public static class GlobalOptions
    {
        [Option("env", "e")]
        [Description("The name of the environment to target.")]
        public static string Environment { get; set; }
    }
}