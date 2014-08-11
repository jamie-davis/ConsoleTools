using System;

namespace ConsoleToolkit.ApplicationStyles
{
    public class ConsoleApplicationRequiresDefaultCommand : Exception
    {
        public ConsoleApplicationRequiresDefaultCommand() : base("A console application must have a default command.")
        {
        }
    }
}