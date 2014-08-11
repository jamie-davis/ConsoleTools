using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class DefaultValueMayOnlyBeSpecifiedForPositionalParameters : Exception
    {
        public BasePositional Positional { get; private set; }

        public DefaultValueMayOnlyBeSpecifiedForPositionalParameters(object currentContext)
            : base("DefaultValue may only be used on positional parameters.")
        {
            Positional = currentContext as BasePositional;
        }
    }
}