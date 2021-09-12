using System;

namespace Vt100.Attributes
{
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