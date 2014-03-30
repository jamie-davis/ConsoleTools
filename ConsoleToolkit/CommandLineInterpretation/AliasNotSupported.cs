using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class AliasNotSupported : Exception
    {
        public AliasNotSupported() : base("Alias names are invalid in the current context.")
        {
            
        }
    }
}