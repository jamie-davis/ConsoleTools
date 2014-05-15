using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// The base class for optional paramters of command configurations.
    /// </summary>
    public abstract class BaseOption : IContext, IOption
    {
        private List<string> _aliases = new List<string>();

        public IEnumerable<string> Aliases { get { return _aliases; } }
        public int ParameterCount { get; set; }
        public string Name { get; set; }

        public abstract void Apply(object command, IEnumerable<string> parameters, out string error);
        public string Description { get; set; }

        public bool IsBoolean { get; internal set; }

        public bool IsShortCircuit { get; set; }

        public void Alias(string alias, IEnumerable<string> existingOptionNames)
        {
            if (alias == Name || existingOptionNames.Contains(alias))
                throw new DuplicateOptionName();
            _aliases.Add(alias);
        }
    }
}