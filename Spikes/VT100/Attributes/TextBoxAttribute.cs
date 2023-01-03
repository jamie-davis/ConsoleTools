using System;

namespace VT100.Attributes
{
    /// <summary>
    /// Indicates that a text box should be used to represent the attributed field's value in the user interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class TextBoxAttribute : Attribute
    {
        public string Caption { get; }

        public TextBoxAttribute(string caption)
        {
            Caption = caption;
        }
    }
}