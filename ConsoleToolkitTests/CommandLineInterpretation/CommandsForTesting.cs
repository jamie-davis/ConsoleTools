using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    namespace CommandsForTesting
    {
        [Command("first")]
        [Description("Description of the first command.")]
        internal class FirstCommand
        {


        }

        [Command("second")]
        [Description("The second command is a command with a number of parameters.")]
        internal class SecondCommand
        {
            [Positional]
            [Description("The date the thing should have.")]
            public string DateOfThing { get; set; }

            [Positional]
            [Description("The number of things that should be.")]
            public string NumeberOfThing { get; set; }
        }

        [Command("fourth")]
        [Description(
            "The fourth command is really complicated with a number of parameters and also options. This is the sort of command that needs lots of text."
            )]
        internal class ThirdCommand
        {
            [Positional]
            [Description("The date this complicates nonsense should be forgotten.")]
            public string Date { get; set; }

            [Positional]
            [Description("The amount of nonsense that the user needs to forget.")]
            public string CrypticNum { get; set; }

            [Option]
            [Description("Use this option to consign this command to history, where it belongs.")]
            public bool Ignore { get; set; }

            [Option]
            [Description("An option with two arguments. The arguments need to be described in the text.")]
            public MoreOption More { get; set; }

            [Validate]
            public string ValidateCommand()
            {
                return "error.";
            }

            public class MoreOption
            {
                [Positional(0)]
                public string MoreString { get; set; }

                [Positional(1)]
                public string MoreInt { get; set; }
            }
        }
    }
}
