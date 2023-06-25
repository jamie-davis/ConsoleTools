using System;

namespace VT100.Attributes
{
    /// <summary>
    /// Indicates that a label should be used to represent the attributed field's value in the user interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LabelAttribute : Attribute
    {
        public string Caption { get; }

        public LabelAttribute(string caption)
        {
            Caption = caption;
        }
    }
}