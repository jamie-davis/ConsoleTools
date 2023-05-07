using System;
using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the foreground colour for an input field or all input fields without a more specific setting.
    /// </summary>
    public class InputForegroundAttribute : BaseColourAttribute
    {
        public InputForegroundAttribute(VtColour colour) : base(colour) { }
    }
}