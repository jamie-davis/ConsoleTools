using System;
using ConsoleToolkit.Annotations;

namespace ConsoleToolkit.CommandLineInterpretation
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