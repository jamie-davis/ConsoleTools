using System;
using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Sets the general foreground colour. This will be used as the setting for any *Foreground attributes that are
    /// not given a more specific setting (e.g. <see cref="InputForegroundAttribute"/> if not specified in its own right.
    /// </summary>
    public class ForegroundAttribute : BaseColourAttribute
    {
        public ForegroundAttribute(VtColour colour) : base(colour) {}
    }
}