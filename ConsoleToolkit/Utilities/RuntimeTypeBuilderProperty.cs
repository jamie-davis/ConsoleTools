using System;

namespace ConsoleToolkit.Utilities
{
    internal class RuntimeTypeBuilderProperty
    {
        internal string Name { get; private set; }
        internal Type Type { get; private set; }

        public RuntimeTypeBuilderProperty(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}