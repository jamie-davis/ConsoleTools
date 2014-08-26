using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a property as an option parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public bool ShortCircuit { get; set; }

        public OptionAttribute()
        {
            
        }

        public OptionAttribute(string shortName)
        {
            ShortName = shortName;
        }

        public OptionAttribute(string longName, string shortName)
        {
            ShortName = shortName;
            LongName = longName;
        }
    }
}