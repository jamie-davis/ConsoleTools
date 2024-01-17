using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    internal static class RegionRectCalculator
    {
        public static (bool hasBorder, Rectangle usableRect) Calculate(int regionColumn, int regionRow, 
            int regionWidth, int regionHeight, LayoutProperties layoutProperties)
        {
            var hasBorder = layoutProperties.HasBorder();

            var width = hasBorder ? regionWidth - 2 : regionWidth;
            var height = hasBorder ? regionHeight - 2 : regionHeight;

            var row = hasBorder ? regionRow + 2 : regionRow + 1;
            var column = hasBorder ? regionColumn + 2 : regionColumn + 1;

            var usableRect = new Rectangle((row, column), (width, height), (row, column));
            return (hasBorder, usableRect);
        }
    }
}