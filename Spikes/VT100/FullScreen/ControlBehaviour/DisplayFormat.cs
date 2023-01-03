using System.Diagnostics;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen.ControlBehaviour
{
    /// <summary>
    /// Format specification for a character in a full screen display
    /// </summary>
    [DebuggerDisplay("Foreground:{Foreground} Background:{Background}")]
    internal struct DisplayFormat
    {
        public VtColour Foreground;
        public VtColour Background;

        private static readonly DisplayFormat Default;

        public bool IsDefault()
        {
            return Foreground == Default.Foreground && Background == Default.Background;
        }

        internal void Apply(DisplayFormat[] array, int start, int length)
        {
            while (length-- > 0 && start < array.Length) array[start++].Apply(this);
        }

        internal void Apply(DisplayFormat displayFormat)
        {
            if (displayFormat.Foreground != VtColour.NoColourChange)
                Foreground = displayFormat.Foreground;
            if (displayFormat.Background != VtColour.NoColourChange)
                Background = displayFormat.Background;
        }
    }
}