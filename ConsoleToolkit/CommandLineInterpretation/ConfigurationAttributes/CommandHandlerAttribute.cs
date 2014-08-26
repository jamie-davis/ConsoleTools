using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a class or a method and indicates that it is a command handler.
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