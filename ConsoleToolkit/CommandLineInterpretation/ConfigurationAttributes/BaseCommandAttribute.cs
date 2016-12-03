using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// The base class for command attributes. This defines the properties that all command attributes must provide.
    /// </summary>
    public abstract class BaseCommandAttribute : Attribute
    {
        /// <summary>
        /// The name of the command. If this is not provided, the command name will be derived from the annotated class name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// This must return true if the command is valid within an interactive session.
        /// </summary>
        public abstract bool ValidInInteractiveSession { get; }

        /// <summary>
        /// This must return true if the command is valid in a non-interactive session.
        /// </summary>
        public abstract bool ValidInNonInteractiveSession { get; }
    }
}