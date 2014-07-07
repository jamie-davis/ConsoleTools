using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a property as a positional parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PositionalAttribute : Attribute
    {
        public PositionalAttribute()
        {
            Index = 0;
        }

        public PositionalAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; set; }
    }
    /// <summary>
    /// This attribute decorates a property as a positional parameter.
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
    /// <summary>
    /// This attribute decorates a property as an option parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public string ShortName { get; set; }
        public string LongName { get; set; }

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
    /// <summary>
    /// This attribute decorates a property as an option parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateAttribute : Attribute
    {
        public ValidateAttribute()
        {
            
        }
    }
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
    /// <summary>
    /// This attribute decorates a property to indicate that it provides the description text for a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CommandDescriptionAttribute : Attribute
    {
    }
    /// <summary>
    /// This attribute decorates a property to indicate that it provides the description text for an option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OptionDescriptionAttribute : Attribute
    {
        public string OptionName { get; set; }

        public OptionDescriptionAttribute(string optionName)
        {
            OptionName = optionName;
        }
    }
    /// <summary>
    /// This attribute decorates a property to indicate that it provides the description text for a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ParameterDescriptionAttribute : Attribute
    {
        public string ParameterName { get; set; }

        public ParameterDescriptionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
    }
    
}
