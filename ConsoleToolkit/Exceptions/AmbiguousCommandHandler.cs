using System;
using ConsoleToolkit.Properties;

namespace ConsoleToolkit.Exceptions
{
    public class AmbiguousCommandHandler : Exception
    {
        public Type Type { [UsedImplicitly] get; private set; }

        public AmbiguousCommandHandler(Type type) : base(string.Format("Multiple command handler methods found in class {0}", type))
        {
            Type = type;
        }
    }
}