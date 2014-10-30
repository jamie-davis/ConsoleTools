using System;

namespace ConsoleToolkit.Exceptions
{
    internal class MultipleHandlersForCommand : Exception
    {
        public Type CommandType { get; private set; }

        public MultipleHandlersForCommand(Type commandType) : base(string.Format("There are multiple handler methods for the command type {0}", commandType))
        {
            CommandType = commandType;
        }
    }
}