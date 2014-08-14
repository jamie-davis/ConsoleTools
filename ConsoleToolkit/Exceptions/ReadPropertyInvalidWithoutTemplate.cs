using System;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkit.Exceptions
{
    /// <summary>
    /// This exception can be thrown by console Read methods.<para/>
    /// When an object contains properties that are <see cref="Read{T}"/> instances, a
    /// template instance must be constructable, or supplied. If no template instance
    /// is available or creatable, this exception will be thrown.
    /// </summary>
    public class ReadPropertyInvalidWithoutTemplate : Exception
    {
        public ReadPropertyInvalidWithoutTemplate() : base("Objects with Read properties cannot be used for input without a template instance.")
        {
            
        }
    }
}