namespace VT100.FullScreen
{
    internal interface IFullScreenConsole
    {
        void Write(string text);
        void Write(char? character);
        void SetCursorPosition(int column, int row);
        int WindowWidth { get; }
        int WindowHeight { get; }
    }
}