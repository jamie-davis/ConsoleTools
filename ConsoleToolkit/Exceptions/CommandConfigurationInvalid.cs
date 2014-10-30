using System;

namespace ConsoleToolkit.Exceptions
{
    public class CommandConfigurationInvalid : Exception
    {
        public CommandConfigurationInvalid() : base("No command or parameter definition found in the configuration.")
        {
            
        }
    }
}