using System;
using ConsoleToolkit.Properties;

namespace ConsoleToolkit.Exceptions
{
    public class CommandAlreadySpecified : Exception
    {
        public string Command { [UsedImplicitly] get; private set; }

        public CommandAlreadySpecified(string command) : base(string.Format("The {0} command has already been configured.", command))
        {
            Command = command;
        }
    }
}