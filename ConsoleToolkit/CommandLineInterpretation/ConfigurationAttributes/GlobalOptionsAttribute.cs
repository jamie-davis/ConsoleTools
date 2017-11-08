using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a class and indicates that it is supplying global options for a command driven application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GlobalOptionsAttribute : Attribute
    {
    }
}