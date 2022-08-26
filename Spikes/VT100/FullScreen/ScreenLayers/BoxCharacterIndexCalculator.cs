namespace VT100.FullScreen.ScreenLayers
{
    internal static class BoxCharacterIndexCalculator
    {
        public static int GetIndex(int x, int y, int width)
        {
            return x + (y * width);
        }
    }
}