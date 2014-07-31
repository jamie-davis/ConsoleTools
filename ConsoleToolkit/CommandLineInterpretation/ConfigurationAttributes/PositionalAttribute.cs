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
    /// <summary>
    /// This attribute decorates a class aand indicates that it is a command.
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

    /// <summary>
    /// This attribute decorates a class and indicates that it is a command handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = false)]
    public class CommandHandlerAttribute : Attribute
    {
        public Type CommandType { get; private set; }

        public CommandHandlerAttribute()
        {
            
        }

        public CommandHandlerAttribute(Type commandType)
        {
            CommandType = commandType;
        }
    }
}
