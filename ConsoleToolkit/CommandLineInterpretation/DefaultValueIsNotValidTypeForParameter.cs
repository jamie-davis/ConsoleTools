using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class DefaultValueIsNotValidTypeForParameter : Exception
    {
        public BasePositional Positional { get; private set; }

        public DefaultValueIsNotValidTypeForParameter(object currentContext)
            : base("Type of DefaultValue is not valid for parameter.")
        {
            Positional = currentContext as BasePositional;
        }
    }
}