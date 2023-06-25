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
    
    internal class LayoutProperties
    {
        public IBorderStyle Border { get; set; }

        public bool HasBorder()
        {
            return Border != null
                   && (Border.TopBorder != BorderType.None
                       || Border.BottomBorder != BorderType.None
                       || Border.LeftBorder != BorderType.None
                       || Border.RightBorder != BorderType.None);
        }
    }
}