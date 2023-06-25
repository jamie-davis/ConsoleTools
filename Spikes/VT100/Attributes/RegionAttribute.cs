using System;

namespace VT100.Attributes
{
    /// <summary>
    /// This indicates that the attributed property contains a type with visual definitions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RegionAttribute : Attribute
    {
    }
}