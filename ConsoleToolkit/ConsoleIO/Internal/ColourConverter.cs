using System;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Translates console colours to and from the control sequence instruction equivalent.
    /// </summary>
    internal static class ColourConverter
    {
        public static char Convert(ConsoleColor colour)
        {
            switch (colour)
            {
                case ConsoleColor.Black:
                    return AdapterConfiguration.Black[0];
                
                case ConsoleColor.DarkBlue:
                    return AdapterConfiguration.DarkBlue[0];
                
                case ConsoleColor.DarkCyan:
                    return AdapterConfiguration.DarkCyan[0];
                
                case ConsoleColor.DarkGray:
                    return AdapterConfiguration.DarkGray[0];
                
                case ConsoleColor.DarkGreen:
                    return AdapterConfiguration.DarkGreen[0];
                
                case ConsoleColor.DarkMagenta:
                    return AdapterConfiguration.DarkMagenta[0];
                
                case ConsoleColor.DarkRed:
                    return AdapterConfiguration.DarkRed[0];
                
                case ConsoleColor.Gray:
                    return AdapterConfiguration.Gray[0];
                
                case ConsoleColor.DarkYellow:
                    return AdapterConfiguration.DarkYellow[0];
                
                case ConsoleColor.Blue:
                    return AdapterConfiguration.Blue[0];
                
                case ConsoleColor.Green:
                    return AdapterConfiguration.Green[0];
                
                case ConsoleColor.Cyan:
                    return AdapterConfiguration.Cyan[0];
                
                case ConsoleColor.Red:
                    return AdapterConfiguration.Red[0];
                
                case ConsoleColor.Magenta:
                    return AdapterConfiguration.Magenta[0];

                case ConsoleColor.Yellow:
                    return AdapterConfiguration.Yellow[0];

                default:
                    return AdapterConfiguration.White[0];
            }
        }

        public static ConsoleColor Convert(string colour)
        {
            if (colour == AdapterConfiguration.Black)
                return ConsoleColor.Black;

            if (colour == AdapterConfiguration.DarkBlue)
                return ConsoleColor.DarkBlue;

            if (colour == AdapterConfiguration.DarkCyan)
                return ConsoleColor.DarkCyan;

            if (colour == AdapterConfiguration.DarkGray)
                return ConsoleColor.DarkGray;

            if (colour == AdapterConfiguration.DarkGreen)
                return ConsoleColor.DarkGreen;

            if (colour == AdapterConfiguration.DarkMagenta)
                return ConsoleColor.DarkMagenta;

            if (colour == AdapterConfiguration.DarkRed)
                return ConsoleColor.DarkRed;

            if (colour == AdapterConfiguration.Gray)
                return ConsoleColor.Gray;

            if (colour == AdapterConfiguration.DarkYellow)
                return ConsoleColor.DarkYellow;

            if (colour == AdapterConfiguration.Blue)
                return ConsoleColor.Blue;

            if (colour == AdapterConfiguration.Green)
                return ConsoleColor.Green;

            if (colour == AdapterConfiguration.Cyan)
                return ConsoleColor.Cyan;

            if (colour == AdapterConfiguration.Red)
                return ConsoleColor.Red;

            if (colour == AdapterConfiguration.Magenta)
                return ConsoleColor.Magenta;

            if (colour == AdapterConfiguration.Yellow)
                return ConsoleColor.Yellow;

            return ConsoleColor.White;
        }
    }
}