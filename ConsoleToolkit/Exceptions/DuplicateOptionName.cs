using System;

namespace ConsoleToolkit.Exceptions
{
    public class DuplicateOptionName : Exception
    {
        public DuplicateOptionName() : base("Duplicate option name specified.")
        {
        }
    }
}