namespace VT100.FullScreen.ScreenLayers
{
    internal static class RectangleReducer
    {
        public static Rectangle Reduce(Rectangle rectangle, int maxWidth, int maxHeight)
        {
            if (rectangle.Width <= maxWidth && rectangle.Height <= maxHeight)
                return rectangle;

            var excessWidth = rectangle.Width > maxWidth;
            var excessHeight = rectangle.Height > maxHeight;
            
            var left = excessWidth ? rectangle.KeyCol - (maxWidth / 2) : rectangle.Left;
            var top = excessHeight ? rectangle.KeyRow - (maxHeight / 2) : rectangle.Top;

            var width = excessWidth ? maxWidth : rectangle.Width;
            var height = excessHeight ? maxHeight : rectangle.Height;
            
            //If the key point is close to an edge, we might have chosen a position for the rectangle that leaves space around it
            if (rectangle.KeyRow - rectangle.Top < rectangle.KeyRow - top) top = rectangle.Top;
            if (rectangle.KeyCol - rectangle.Left < rectangle.KeyCol - left) left = rectangle.Left;
            if (rectangle.Left + rectangle.Width < left + maxWidth) left = rectangle.Left + rectangle.Width - maxWidth;
            if (rectangle.Top + rectangle.Height < top + maxHeight) top = rectangle.Top + rectangle.Height - maxHeight;
            
            return new Rectangle((left, top), (width, height), (rectangle.KeyCol, rectangle.KeyRow));
        }
    }
}