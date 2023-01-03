using System;
using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the foreground colour for an input field or all input fields without a more specific setting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    [PropertyAttribute]
    internal class InputForegroundAttribute : Attribute
    {
        public VtColour Colour { get; }

        public InputForegroundAttribute(VtColour unknown)
        {
            Colour = unknown;
        }
    }
}