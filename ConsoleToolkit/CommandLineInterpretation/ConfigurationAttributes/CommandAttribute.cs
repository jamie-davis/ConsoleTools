using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a class and indicates that it is a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }

        public CommandAttribute(string name)
        {
            Name = name;
        }

        public CommandAttribute()
        {
            
        }
    }
}