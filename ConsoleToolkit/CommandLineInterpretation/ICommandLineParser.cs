using System.Collections.Generic;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// Implement this interface to supply a command line parser. The <see cref="Parse"/> method will be called with 
    /// the command line arguments and lists of the options and positional parameters defined for the application.
    /// 
    /// The positional parameter and option lists are supplied for reference purposes. It is not an error for the parser
    /// to return parameters and options not in the list if they can be identified as such from the arguments array.
    /// The framework will perform conversions from text to the configured types, and is responsible for the detection and 
    /// reporting of format errors and parameter counts. The command line parser implementation is only responsible for 
    /// extracting the text values.
    /// 
    /// If command line convention allows positional parameters to be named and given out of order, the parser can add them
    /// as if they were options. The framework will take this into account and will not attempt to match those parameters
    /// from the positional arguments reported.
    /// </summary>
    public interface ICommandLineParser
    {
        /// <summary>
        /// Parse an array of command line arguments. Use the supplied <see cref="IParserResult"/> interface to report positional
        /// arguments and options extracted from the command line arguments.
        /// 
        /// The parse method is not required to validate that options are valid, invalid ones should be extracted so that the framework
        /// can see them. The method is also not required to validate that all positional arguments have been supplied, it need only report
        /// the arguments that were supplied and were not options. The framework will validate that all of the required arguments are present
        /// and that all arguments are of the expected type.
        /// </summary>
        /// <param name="args">The array of arguments to be parsed.</param>
        /// <param name="options">The configured options that may be specified.</param>
        /// <param name="positionalArguments">The positional arguments that may be specified.</param>
        /// <param name="result">An interface allowing parse results to be reported.</param>
        void Parse(string[] args, IEnumerable<IOption> options, IEnumerable<IPositionalArgument> positionalArguments, IParserResult result);
    }
}