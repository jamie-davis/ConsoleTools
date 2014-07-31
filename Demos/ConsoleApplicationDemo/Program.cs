using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleApplicationDemo
{
    class Program : ConsoleApplication
    {
        static void Main(string[] args)
        {
            Toolkit.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            Toolkit.Execute(args);
        }

        protected override void Initialise()
        {
            HelpOption<Options>(o => o.ShowHelp);
            base.Initialise();
        }
    }

    [Command]
    [Description("This program illustrates a simple console application. The default command is defined using attributes, and help is provided automatically.")]
    public class Options
    {
        [Positional("P1")]
        [Description("The first parameter.")]
        public string Pos1 { get; set; }

        [Option("help", "h", ShortCircuit = true)]
        [Description("Display this help text.")]
        public bool ShowHelp { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine("You said: {0}", Pos1);
        }
    }
}
