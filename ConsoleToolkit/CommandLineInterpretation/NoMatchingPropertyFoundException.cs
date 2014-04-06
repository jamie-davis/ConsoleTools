using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class NoMatchingPropertyFoundException : Exception
    {
        public string PropertyName { get; private set; }
        public Type CommandType { get; private set; }

        public NoMatchingPropertyFoundException(string propertyName, Type commandType) : base(string.Format("Type {0} contains no matching property for parameter named {1}", commandType.Name, propertyName))
        {
            PropertyName = propertyName;
            CommandType = commandType;
        }
    }
}