using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class ShortCircuitInvalid : Exception
    {
        public ShortCircuitInvalid() : base ("The short circuit option cannot be used here.")
        {
            
        }
    }
}