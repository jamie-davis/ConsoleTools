using System.Diagnostics;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal static class BuiltInCommandLineParsers
    {
        public static ICommandLineParser Get(CommandLineParserConventions convention)
        {
            switch (convention)
            {
                case CommandLineParserConventions.MicrosoftStandard:
                    return new MicrosoftStandardCommandLineParser();

                case CommandLineParserConventions.MsDosConventions:
                    return new MsDosCommandLineParser();

                case CommandLineParserConventions.PosixConventions:
                    return new PosixCommandLineParser();

                default:
                    Debug.Assert(false, "Internal error: unknown command line parser selected.");
                    return null;
            }
        }
    }
}