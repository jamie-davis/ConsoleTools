using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the foreground colour for a label or all label fields without a more specific setting.
    /// </summary>
    public class LabelForegroundAttribute : BaseColourAttribute
    {
        public LabelForegroundAttribute(VtColour colour) : base(colour) { }
    }
}