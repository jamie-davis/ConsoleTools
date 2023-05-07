using System;

namespace VT100.Attributes
{
    /// <summary>
    /// Indicates that a format property should be defaulted from another property type if no value is given.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class DefaultFromAttribute : Attribute
    {
        public Type AttributeToUseForDefault { get; }

        public DefaultFromAttribute(Type attributeToUseForDefault)
        {
            AttributeToUseForDefault = attributeToUseForDefault;
        }
    }
}