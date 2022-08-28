using System;

namespace VT100.FullScreen
{
    internal class DefaultFullScreenConsole : IFullScreenConsole
    {
        #region Implementation of IFullScreenConsole

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void Write(char? character)
        {
            Console.Write(character ?? ' ');
        }

        public void SetCursorPosition(int column, int row)
        {
            Console.SetCursorPosition(column, row);
        }

        public int WindowWidth => Console.WindowWidth;
        public int WindowHeight => Console.WindowHeight;

        #endregion
    }
}