using System;

namespace ConsoleToolkit.Exceptions
{
    internal class CommandHandlerDoesNotHaveAttribute : Exception
    {
        public Type Type { get; private set; }

        public CommandHandlerDoesNotHaveAttribute(Type type) : base(string.Format("{0} does not have a command handler attribute", type.Name))
        {
            Type = type;
        }
    }
}