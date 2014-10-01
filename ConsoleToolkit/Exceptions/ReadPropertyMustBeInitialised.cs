using System;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Properties;

namespace ConsoleToolkit.Exceptions
{
    /// <summary>
    /// This exception can be thrown by console Read methods.<para/>
    /// When an object contains properties that are <see cref="Read{T}"/> instances, the
    /// properties must have values in the template instance. 
    /// </summary>
    public class ReadPropertyMustBeInitialised : Exception
    {
        public string PropertyName { [UsedImplicitly] get; private set; }

        public ReadPropertyMustBeInitialised(string propertyName) : base(string.Format("The {0} property must have a templated value.", propertyName))
        {
            PropertyName = propertyName;
        }
    }
}