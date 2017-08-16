using System.Collections.Generic;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal interface ICommandKeys
    {
        List<string> Keywords { get; }
        string Name { get; }
    }
}