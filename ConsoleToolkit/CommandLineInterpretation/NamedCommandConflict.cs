using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class NamedCommandConflict : Exception
    {
        public NamedCommandConflict()
            : base("Program parameters and named commands cannot be configured at the same time.")
        {
        }
    }
}