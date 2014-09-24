using System;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkitTests.TestingUtilities
{
    static class CommandConfigDescriber
    {
        public static void Describe(CommandLineInterpreterConfiguration config, ConsoleAdapter console, string applicationName, CommandLineParserConventions conventions)
        {
            CommandDescriber.Describe(config, console, applicationName);

            var adorner = MakeAdorner(conventions);
            foreach (var baseCommandConfig in config.Commands)
            {
                console.WriteLine();
                console.WriteLine("Description of {0} command:", baseCommandConfig.Name);
                CommandDescriber.Describe(baseCommandConfig, console, adorner);
            }
        }

        private static IOptionNameHelpAdorner MakeAdorner(CommandLineParserConventions conventions)
        {
            switch (conventions)
            {
                case CommandLineParserConventions.MicrosoftStandard:
                    return new MicrosoftStandardCommandLineParser();
                case CommandLineParserConventions.MsDosConventions:
                    return new MsDosCommandLineParser();
                case CommandLineParserConventions.PosixConventions:
                    return new PosixCommandLineParser();
                default:
                    throw new ArgumentException(string.Format("Only built in conventions are supported."));
            }
        }
    }
}
