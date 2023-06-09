using System;

namespace VT100.Attributes
{
    /// <summary>
    /// Indicates that this attribute is a setting that changes the behaviour or appearance of a control and should be
    /// included in control property sets. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class PropertyAttributeAttribute : Attribute
    {
    }
}