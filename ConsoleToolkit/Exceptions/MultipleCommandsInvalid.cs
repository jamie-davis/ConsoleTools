using System;

namespace ConsoleToolkit.Exceptions
{
    public class MultipleCommandsInvalid : Exception
    {
        public MultipleCommandsInvalid() : base("ConsoleApplication derived classes must not have multiple commands defined.")
        {
            
        }
    }
}