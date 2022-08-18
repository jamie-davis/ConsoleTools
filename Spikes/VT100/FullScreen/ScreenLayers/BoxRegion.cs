using VT100.FullScreen.ControlBehaviour;

namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// Immutable box region class. This describes a box region in terms of size and position, and also including
    /// a display format to describe the visual characteristics required. Boxes are outlines, intended to be rendered
    /// as a layer above or below other content, and as such do not carry any content information.
    /// </summary>
    internal sealed class BoxRegion
    {
        public BoxRegion(int x, int y, int width, int height, DisplayFormat format = default)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Format = format;
        }

        internal int X { get; }
        internal int Y { get; }
        internal int Width { get; }
        internal int Height { get; }
        internal DisplayFormat Format { get; }
    }
}