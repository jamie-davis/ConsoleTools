using System;

namespace ConsoleToolkit.Exceptions
{
    public class AliasNotSupported : Exception
    {
        public AliasNotSupported() : base("Alias names are invalid in the current context.")
        {
            
        }
    }
}