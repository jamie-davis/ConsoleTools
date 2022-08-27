namespace VT100.FullScreen.ScreenLayers
{
    internal static class CharacterArrayIndexCalculator
    {
        public static int GetIndex(int x, int y, int width)
        {
            return x + (y * width);
        }
    }
}