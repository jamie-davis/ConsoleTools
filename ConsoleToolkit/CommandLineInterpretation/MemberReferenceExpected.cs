using System;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class MemberReferenceExpected : Exception
    {
        public MemberReferenceExpected() : base("Expected a member reference expression to be specified.")
        {
            
        }
    }
}