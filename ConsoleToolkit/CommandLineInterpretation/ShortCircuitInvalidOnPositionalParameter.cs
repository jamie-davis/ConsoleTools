using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class ShortCircuitInvalidOnPositionalParameter : Exception
    {
        public BasePositional Positional { get; private set; }

        public ShortCircuitInvalidOnPositionalParameter(object currentContext) : base ("ShortCircuitOption cannot be used on positional parameter.")
        {
            Positional = currentContext as BasePositional;
        }
    }
}