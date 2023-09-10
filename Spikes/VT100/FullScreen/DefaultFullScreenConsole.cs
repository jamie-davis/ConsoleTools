using System;
using VT100.Attributes;
using VT100.FullScreen.ControlBehaviour;
using VT100.Utilities;

namespace VT100.FullScreen
{
    internal class DefaultFullScreenConsole : IFullScreenConsole
    {
        public DefaultFullScreenConsole()
        {
            BackgroundColour = ConsoleToVtColour.Convert(Console.BackgroundColor);
            ForegroundColour = ConsoleToVtColour.Convert(Console.ForegroundColor);
        }

        public VtColour BackgroundColour { get; set; }
        public VtColour ForegroundColour { get; set; }

        #region Implementation of IFullScreenConsole

        public void Write(string text, DisplayFormat format = default)
        {
            Console.Write($"{ColourAttribute.GetColourAttribute(format)}{text}");
        }

        public void Write(char? character, DisplayFormat format = default)
        {
            Console.Write($"{ColourAttribute.GetColourAttribute(format)}{character ?? ' '}");
        }

        public void SetCursorPosition(int column, int row)
        {
            Console.SetCursorPosition(column, row);
        }

        public int WindowWidth => Console.WindowWidth;
        public int WindowHeight => Console.WindowHeight;
        public void SetCharacter(int column, int row, char character, DisplayFormat format)
        {
            if (column >= WindowHeight || column < 0 || row >= WindowWidth || row < 0) return;
            using (new CursorHider())
            {
                var oldPosX = Console.CursorLeft;
                var oldPosY = Console.CursorTop;

                Console.SetCursorPosition(column, row);
                Console.Write(character);

                Console.SetCursorPosition(oldPosX, oldPosY);
            }
        }

        #endregion
    }
}