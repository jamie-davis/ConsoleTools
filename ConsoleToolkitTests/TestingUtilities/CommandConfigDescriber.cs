using System;
using System.Linq;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkitTests.TestingUtilities
{
    static class CommandConfigDescriber
    {
        public static void Describe(CommandLineInterpreterConfiguration config, ConsoleAdapter console, string applicationName, CommandLineParserConventions conventions, CommandExecutionMode executionMode)
        {
            CommandDescriber.Describe(config, console, applicationName, executionMode);

            var adorner = MakeAdorner(conventions);
            foreach (var baseCommandConfig in config.Commands)
            {
                console.WriteLine();
                console.WriteLine("Description of {0} command:", string.Join(" ", baseCommandConfig.Keywords.Concat(new [] { baseCommandConfig.Name })));
                CommandDescriber.Describe(baseCommandConfig, config, console, executionMode, adorner);
            }
        }

        public static void DescribeKeywords(string keyword, CommandLineInterpreterConfiguration config, ConsoleAdapter console)
        {
            CommandDescriber.DescribeKeywords(config.Commands, new [] {keyword}, console);
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
