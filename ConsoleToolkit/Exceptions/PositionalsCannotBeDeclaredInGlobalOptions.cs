using System;

namespace ConsoleToolkit.Exceptions
{
    class PositionalsCannotBeDeclaredInGlobalOptions : Exception
    {
        public PositionalsCannotBeDeclaredInGlobalOptions(Type type) : base($"{type.Name} contains global options but also declares positionals, which cannot be created globally.")
        {
            
        }
    }
}
