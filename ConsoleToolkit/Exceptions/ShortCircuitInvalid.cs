using System;

namespace ConsoleToolkit.Exceptions
{
    public class ShortCircuitInvalid : Exception
    {
        public ShortCircuitInvalid() : base ("The short circuit option cannot be used here.")
        {
            
        }
    }
}