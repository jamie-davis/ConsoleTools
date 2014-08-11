using System;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal class CommandsMayOnlyDeclareOneHandlerMethod : Exception
    {
        public Type CommandType { get; private set; }

        public CommandsMayOnlyDeclareOneHandlerMethod(Type commandType) : base(string.Format("The command type {0} declares multiple handler methods.", commandType))
        {
            CommandType = commandType;
        }
    }
}