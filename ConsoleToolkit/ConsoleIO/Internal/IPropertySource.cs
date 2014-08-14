using System.Reflection;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Defines a property source
    /// </summary>
    internal interface IPropertySource
    {
        PropertyInfo Property { get; }
        object Value { get; }
    }
}