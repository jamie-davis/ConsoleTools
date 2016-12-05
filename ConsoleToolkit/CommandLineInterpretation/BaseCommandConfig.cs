using System;
using System.Collections.Generic;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// The base class for command configurations.
    /// </summary>
    public abstract class BaseCommandConfig : IContext
    {
        public Type CommandType { get; set; }
        public string Name { get; set; }
        public List<BasePositional> Positionals = new List<BasePositional>();
        public List<BaseOption> Options = new List<BaseOption>();

        internal abstract object Create(string commandName);
        internal abstract bool Validate(object command, IList<string> messages);
        string IContext.Description { get; set; }

        /// <summary>
        /// True if this command is valid to invoke from command line parameters
        /// </summary>
        public bool ValidInNonInteractiveContext { get; set; }

        /// <summary>
        /// True if this command is valid to invoke from an interactive session
        /// </summary>
        public bool ValidInInteractiveContext { get; set; }

    }
}