using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a method and indicates that it is a command validator method. The framework will
    /// call attrbuted method to validate a command once it has populated its properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandValidatorAttribute : Attribute
    {
    }
}
