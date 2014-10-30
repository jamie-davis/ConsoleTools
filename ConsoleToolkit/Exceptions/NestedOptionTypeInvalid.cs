using System;

namespace ConsoleToolkit.Exceptions
{
    public class NestedOptionTypeInvalid : Exception
    {
        public Type NestedType { get; private set; }

        public NestedOptionTypeInvalid(string error, Type nestedType) : base(error)
        {
            NestedType = nestedType;
        }
    }
}