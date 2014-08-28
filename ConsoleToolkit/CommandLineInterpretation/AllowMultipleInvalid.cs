using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class AllowMultipleInvalid : Exception
    {
        public AllowMultipleInvalid() : base ("The allow multiple option cannot be used here.")
        {
            
        }
    }
}