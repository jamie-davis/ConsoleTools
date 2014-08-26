using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute provides a literal description string for a command, positional parameter or option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Text { get; set; }

        public DescriptionAttribute(string text)
        {
            Text = text;
        }
    }
}