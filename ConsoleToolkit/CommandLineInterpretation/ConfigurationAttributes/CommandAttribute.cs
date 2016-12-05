using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a class and indicates that it is a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute : BaseCommandAttribute
    {
        /// <inheritdoc />
        public override bool ValidInInteractiveSession { get { return true; } }

        /// <inheritdoc />
        public override bool ValidInNonInteractiveSession { get { return true; } }

        /// <summary>
        /// Constructor specifying a command name
        /// </summary>
        public CommandAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructor specifying that the command name should be derived
        /// </summary>
        public CommandAttribute()
        {
            
        }
    }
}