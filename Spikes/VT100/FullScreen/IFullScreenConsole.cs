using System.Reflection.PortableExecutable;
using VT100.FullScreen.ControlBehaviour;

namespace VT100.FullScreen
{
    internal interface IFullScreenConsole
    {
        void Write(string text, DisplayFormat format = default);
        void Write(char? character, DisplayFormat format = default);
        void SetCursorPosition(int column, int row);
        int WindowWidth { get; }
        int WindowHeight { get; }
        void SetCharacter(int column, int row, char character, DisplayFormat format);
    }
}