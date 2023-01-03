using System;

namespace VT100.Attributes
{
    /// <summary>
    /// Indicates that this attribute us a setting that changes the behaviour or appearance of a control and should be
    /// included in control property sets. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class PropertyAttributeAttribute : Attribute
    {
    }
}