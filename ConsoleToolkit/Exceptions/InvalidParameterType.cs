using System;

namespace ConsoleToolkit.Exceptions
{
    /// <summary>
    /// This exception is thrown when a parameter is specified with an unsupported type.
    /// </summary>
    public class InvalidParameterType : Exception
    {
        public Type InvalidParamType { get; set; }

        public InvalidParameterType(Type invalidParamType) : base(string.Format("Invalid parameter type {0}", invalidParamType.Name))
        {
            InvalidParamType = invalidParamType;
        }
    }
}