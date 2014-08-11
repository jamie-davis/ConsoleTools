using System;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal class ParameterTypeCannotBeResolved : Exception
    {
        public Type ParameterType { get; set; }

        public ParameterTypeCannotBeResolved(Type parameterType) : base(string.Format("A parameter of type {0} cannot be injected.", parameterType))
        {
            ParameterType = parameterType;
        }
    }
}