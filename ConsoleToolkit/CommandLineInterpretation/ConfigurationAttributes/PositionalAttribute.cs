using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a property as a positional parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PositionalAttribute : Attribute
    {
        private string _defaultValue;
        private bool _defaultSpecified = false;

        public PositionalAttribute()
        {
            Index = 0;
        }

        public PositionalAttribute(int index) : this(null, index)
        {
        }

        public PositionalAttribute(string name, int index = 0)
        {
            Index = index;
            Name = name;
        }

        public string Name { get; set; }

        public int Index { get; set; }

        public string DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                _defaultSpecified = true;
            }
        }

        public bool DefaultSpecified { get { return _defaultSpecified; } }
    }
}
