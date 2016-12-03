using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a class and indicates that it is a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NonInteractiveCommandAttribute : BaseCommandAttribute
    {
        /// <inheritdoc />
        public override bool ValidInInteractiveSession => false;

        /// <inheritdoc />
        public override bool ValidInNonInteractiveSession => true;

        /// <summary>
        /// Constructor specifying a command name
        /// </summary>
        public NonInteractiveCommandAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructor specifying that the command name should be derived
        /// </summary>
        public NonInteractiveCommandAttribute()
        {
            
        }
    }
}