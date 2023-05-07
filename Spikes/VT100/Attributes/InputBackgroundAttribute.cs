using System;
using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the background colour for an input field or all input fields without a more specific setting.
    /// </summary>
    public class InputBackgroundAttribute : BaseColourAttribute
    {
        public InputBackgroundAttribute(VtColour colour) : base(colour){}
    }
}