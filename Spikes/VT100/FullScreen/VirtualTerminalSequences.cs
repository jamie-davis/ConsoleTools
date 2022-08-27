namespace VT100.FullScreen
{
    internal class VirtualTerminalSequences
    {
        //Functional
        public static char ESC = '\x1b';
        public static char CSIChar = '[';
        public static char NUL = '\0';
        public static string CSI = $"{ESC}{CSIChar}";

        //Screen buffer
        public static string UseAlternateScreenBuffer = $"{CSI}?1049h";
        public static string UseMainScreenBuffer = $"{CSI}?1049l";

        //Cursor Visibility
        public static string HideCursor = $"{CSI}?25l";
        public static string ShowCursor = $"{CSI}?25h";

        //Character attributes
        public static string UnderlineMode = $"{CSI}4m";

        public static string SingleHeight = $"{ESC}#4";

        //Screen control
        public static string ClearScreen = $"{CSI}2J";

        public static string SetColour()
        {
            return $"{CSI}";
        }
    }
}