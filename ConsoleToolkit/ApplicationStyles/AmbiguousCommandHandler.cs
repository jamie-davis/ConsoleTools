using System;

namespace ConsoleToolkit.ApplicationStyles
{
    public class AmbiguousCommandHandler : Exception
    {
        public Type Type { get; private set; }

        public AmbiguousCommandHandler(Type type) : base(string.Format("Multiple command handler methods found in class {0}", type))
        {
            Type = type;
        }
    }
}