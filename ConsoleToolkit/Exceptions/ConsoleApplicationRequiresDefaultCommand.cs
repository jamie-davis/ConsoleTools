using System;

namespace ConsoleToolkit.Exceptions
{
    public class ConsoleApplicationRequiresDefaultCommand : Exception
    {
        public ConsoleApplicationRequiresDefaultCommand() : base("A console application must have a default command.")
        {
        }
    }
}