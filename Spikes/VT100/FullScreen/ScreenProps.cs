using VT100.FullScreen.ControlBehaviour;

namespace VT100.FullScreen
{
    internal class ScreenProps
    {
        public VtColour Background { get; set; } = VtColour.NoColourChange;
        public VtColour Foreground { get; set; } = VtColour.NoColourChange;

        public DisplayFormat MakeBaseFormat()
        {
            return new DisplayFormat
            {
                Foreground = Foreground,
                Background = Background
            };
        }
    }
}