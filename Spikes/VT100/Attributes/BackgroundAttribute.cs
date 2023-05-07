using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the general background colour. This will be used for the empty space inside the attributed object,
    /// and as the setting for any *Background attributes that are not given a more specific setting (e.g.
    /// <see cref="InputBackgroundAttribute"/> if not specified in its own right.
    /// </summary>
    public class BackgroundAttribute : BaseColourAttribute
    {
        public BackgroundAttribute(VtColour colour) : base(colour) {}
    }
}