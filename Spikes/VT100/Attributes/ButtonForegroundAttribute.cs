using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the foreground colour for a button or all buttons without a more specific setting.
    /// </summary>
    public class ButtonForegroundAttribute : BaseColourAttribute
    {
        public ButtonForegroundAttribute(VtColour colour) : base(colour) { }
    }
}