namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// This interface is passed to command line parser implementations to allow parsed arguments to be provided to the framework.
    /// 
    /// Parser implementations are not responsible for validation of data types or alerting when arguments are missing as these functions
    /// are carried out by the framework. 
    /// </summary>
    public interface IParserResult
    {
        /// <summary>
        /// Call this when a named option is extracted from the command line.
        /// 
        /// Please also use this call for a positional parameter if the parsing conventions allow positional parameters to be identified by name.
        /// </summary>
        /// <param name="optionName">The name of the option from the supplied list of options.</param>
        /// <param name="arguments">The arguments supplied for the option. This will be validated by the framework. 
        ///     If the command line conventions implemented by the parser do no allow precise identification of the number 
        ///     of parameters supplied by the user,  just accept what is present up to the maximum. If the conventions allow precise 
        ///     identification of the option parameters, please supply them all.</param>
        ParseOutcome OptionExtracted(string optionName, string[] arguments);

        /// <summary>
        /// This call should only be used if the parsing conventions do not allow identification of positional parameters by name. Call this
        /// method with each positional parameter in turn.
        /// </summary>
        /// <param name="value"></param>
        ParseOutcome PositionalArgument(string value);
    }
}