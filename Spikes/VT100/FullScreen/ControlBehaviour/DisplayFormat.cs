using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen.ControlBehaviour
{
    /// <summary>
    /// Format specification for a character in a full screen display
    /// </summary>
    internal struct DisplayFormat
    {
        public VtColour Colour;

        private static readonly DisplayFormat Default;

        public bool IsDefault()
        {
            return Colour == Default.Colour;
        }

        internal void Apply(DisplayFormat[] array, int start, int length)
        {
            while (length-- > 0 && start < array.Length) array[start++].Apply(this);
        }

        internal void Apply(DisplayFormat displayFormat)
        {
            if (displayFormat.Colour != VtColour.NoColourChange)
                Colour = displayFormat.Colour;
        }
    }
}