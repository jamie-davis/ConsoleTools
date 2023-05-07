using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the background colour for a button or all buttons without a more specific setting.
    /// </summary>
    public class ButtonBackgroundAttribute : BaseColourAttribute
    {
        public ButtonBackgroundAttribute(VtColour colour) : base(colour){}
    }
}