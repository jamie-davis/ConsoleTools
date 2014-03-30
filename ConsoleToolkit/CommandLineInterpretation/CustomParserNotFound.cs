using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class CustomParserNotFound : Exception
    {
        public CustomParserNotFound() : base("A custom command line parser was selected but no custom parser has been supplied.")
        {
            
        }
    }
}