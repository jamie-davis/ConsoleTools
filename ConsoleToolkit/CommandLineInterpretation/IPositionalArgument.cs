namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// A positional argument. If positional arguments cannot be referenced by name, the command line parser may ignore this
    /// interface.
    /// 
    /// The Microsoft Command Line Standard guidelines allow this simply by referring to a positional parameter by name as
    /// if it were an option. If the convention being implemented allows this, supply the positional argument as if it 
    /// was actually an option. The framework will recognise that this has happened and will not attempt to extract a
    /// value for the argument from the positional parameters parsed from the command line.
    /// </summary>
    public interface IPositionalArgument
    {
        /// <summary>
        /// The argument name. Useful only if conventions allow the user to specify values for positional parameters as if they 
        /// were options.
        /// </summary>
        string ParameterName { get; }
    }
}