using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// The base class for positional parameters of command configurations.
    /// </summary>
    public abstract class BasePositional : IContext, IPositionalArgument
    {
        public string ParameterName { get; set; }
        public abstract Type ParameterType { get; }

        protected BasePositional(string parameterName)
        {
            ParameterName = parameterName;
        }

        public abstract string Accept(object command, string value);
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public bool IsOptional { get; set; }
    }
}