using System;
using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the background colour for an input field or all input fields without a more specific setting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    [PropertyAttribute]
    internal class InputBackgroundAttribute : Attribute
    {
        public VtColour Colour { get; }
        public InputBackgroundAttribute(VtColour colour)
        {
            Colour = colour;
        }
    }
}