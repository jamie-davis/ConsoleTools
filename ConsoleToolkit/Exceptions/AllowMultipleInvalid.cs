using System;

namespace ConsoleToolkit.Exceptions
{
    public class AllowMultipleInvalid : Exception
    {
        public AllowMultipleInvalid() : base ("The allow multiple option cannot be used here.")
        {
            
        }
    }
}