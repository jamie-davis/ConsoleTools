using System;
using System.Reflection.Metadata.Ecma335;
using VT100.FullScreen;

namespace VT100.Utilities
{
    internal static class ConsoleToVtColour
    {
        public static VtColour Convert(ConsoleColor colour)
        {
            switch (colour)
            {
                case ConsoleColor.Black:
                    return VtColour.Black;
                case ConsoleColor.DarkBlue:
                    return VtColour.Blue;
                case ConsoleColor.DarkGreen:
                    return VtColour.Green;
                case ConsoleColor.DarkCyan:
                    return VtColour.Cyan;
                case ConsoleColor.DarkRed:
                    return VtColour.Red;
                case ConsoleColor.DarkMagenta:
                    return VtColour.Magenta;
                case ConsoleColor.DarkYellow:
                    return VtColour.Yellow;
                case ConsoleColor.Gray:
                    return VtColour.White;
                case ConsoleColor.DarkGray:
                    return VtColour.BrightBlack;
                case ConsoleColor.Blue:
                    return VtColour.BrightBlue;
                case ConsoleColor.Green:
                    return VtColour.BrightGreen;
                case ConsoleColor.Cyan:
                    return VtColour.BrightCyan;
                case ConsoleColor.Red:
                    return VtColour.BrightRed;
                case ConsoleColor.Magenta:
                    return VtColour.BrightMagenta;
                case ConsoleColor.Yellow:
                    return VtColour.BrightYellow;
                case ConsoleColor.White:
                    return VtColour.BrightWhite;
                default:
                    throw new ArgumentOutOfRangeException(nameof(colour), colour, null);
            }
        }
    }
}