using System;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace ConsoleToolkit.Exceptions
{
    /// <summary>
    /// This error is thrown when an interactive help command type is nominated, but the type does not have the
    /// <see cref="InteractiveCommandAttribute"/>.
    /// </summary>
    public class InvalidInteractiveHelpCommand : Exception
    {
        public InvalidInteractiveHelpCommand(string error) : base(error)
        {
            
        }
    }
}
