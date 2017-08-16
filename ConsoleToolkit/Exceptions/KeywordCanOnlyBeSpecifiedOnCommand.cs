using System;

namespace ConsoleToolkit.Exceptions
{
    public class KeywordCanOnlyBeSpecifiedOnCommand : Exception
    {
        public KeywordCanOnlyBeSpecifiedOnCommand() : base("Keywords can only be specified on commands.")
        {
            
        }
    }
}