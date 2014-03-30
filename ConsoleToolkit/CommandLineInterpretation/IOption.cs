namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// An option configured as valid. The information supplied here is meant to guide the parser, and depending on the 
    /// conventions being implemented, may not be required.
    /// 
    /// An example of when this information is useful is in the POSIX conventions, in which option arguments can be attached to 
    /// the option itself. e.g. "program -nabc" in which the -n option has an argument of "abc". Equally, if the available options
    /// listed options "n", "a", "b" and "c" as boolean (i.e. no parameters), "program -nabc" would be setting "n", "a", "b", and "c" 
    /// on. Without a list of configured options it would not be possible for the parser to distinguish "stacked" boolean options from
    /// an option plus its parameter.
    /// </summary>
    public interface IOption
    {
        /// <summary>
        /// The name of the option.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// True if the option is a boolean flag - i.e. specifying the option sets it to TRUE.
        /// </summary>
        bool IsBoolean { get; }

        /// <summary>
        /// The number of parameters required by the option. If conventions allow the number of parameters specified by the
        /// user to be determined, then this value can be ignored. It is not intended to support validation of option arguments,
        /// as the framework has this responsibilty.
        /// </summary>
        int ParameterCount { get; }
    }
}