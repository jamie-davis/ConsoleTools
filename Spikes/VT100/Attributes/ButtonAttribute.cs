using System;

namespace VT100.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class ButtonAttribute : Attribute
    {
        public string Caption { get; }
        public ExitMode ExitMode { get; }

        public ButtonAttribute(string caption, ExitMode exitMode = ExitMode.DoesNotExit)
        {
            Caption = caption;
            ExitMode = exitMode;
        }
    }
}