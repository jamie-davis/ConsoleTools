using VT100.FullScreen.ControlBehaviour;

namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// Immutable box region class. This describes a box region in terms of size and position, and also including
    /// the visual characteristics required. Boxes are outlines, intended to be rendered as a layer above or
    /// below other content, and as such do not carry any content information.
    /// </summary>
    internal sealed class BoxRegion
    {
        public BoxRegion(int x, int y, int width, int height, LineWeight lineWeight = LineWeight.Light, LineCount lineCount = LineCount.Single, DashType dashType = DashType.None, CornerType cornerType = CornerType.Box, DisplayFormat format = default)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            LineWeight = lineWeight;
            LineCount = lineCount;
            DashType = dashType;
            CornerType = cornerType;
            Format = format;
        }

        internal int X { get; }
        internal int Y { get; }
        internal int Width { get; }
        internal int Height { get; }
        public LineWeight LineWeight { get; }
        public LineCount LineCount { get; }
        public DashType DashType { get; }
        public CornerType CornerType { get; }
        internal DisplayFormat Format { get; }
    }
}