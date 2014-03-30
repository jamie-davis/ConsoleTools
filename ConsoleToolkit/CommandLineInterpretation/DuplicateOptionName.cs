using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class DuplicateOptionName : Exception
    {
        public DuplicateOptionName() : base("Duplicate option name specified.")
        {
        }
    }
}