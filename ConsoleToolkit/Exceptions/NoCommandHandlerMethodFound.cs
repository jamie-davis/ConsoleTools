using System;

namespace ConsoleToolkit.Exceptions
{
    public class NoCommandHandlerMethodFound : Exception
    {
        public Type Type { get; private set; }

        public NoCommandHandlerMethodFound(Type type) : base(string.Format("No command handler found in type {0}", type))
        {
            Type = type;
        }
    }
}