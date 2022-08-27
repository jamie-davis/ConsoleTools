using System;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// The global adapter configuration.
    /// </summary>
    internal static class AdapterConfiguration
    {
        internal const string PrefixAffinity = ">";
        internal const string SuffixAffinity = "<";
        internal const string PushInstruction = "P";
        internal const string PopInstruction = "p";
        internal const string SetForegroundInstruction = "S";
        internal const string SetBackgroundInstruction = "s";
        internal const string Black = "k";
        internal const string DarkBlue = "B";
        internal const string DarkGreen = "G";
        internal const string DarkCyan = "C";
        internal const string DarkRed = "R";
        internal const string DarkMagenta = "M";
        internal const string DarkYellow = "Y";
        internal const string Gray = "x";
        internal const string DarkGray = "X";
        internal const string Blue = "b";
        internal const string Green = "g";
        internal const string Cyan = "c";
        internal const string Red = "r";
        internal const string Magenta = "m";
        internal const string Yellow = "y";
        internal const string White = "w";

        private static char _controlSequenceIntroducer;
        private static char _controlSequenceTerminator;

        #region Internal mechanism to publish CSI and CST changes 
        internal class ControlSequenceUpdateEventArgs {}
        internal delegate void ControlSequenceUpdateEventHandler(object sender, ControlSequenceUpdateEventArgs e);

        internal static event ControlSequenceUpdateEventHandler ControlSequenceUpdate;

        private static void RaiseControlSequenceUpdate()
        {
            var handler = ControlSequenceUpdate;
            if (handler != null) handler(null, new ControlSequenceUpdateEventArgs());
        }
        #endregion

        /// <summary>
        /// Defines the character that introduces a control sequence in console output.
        /// </summary>
        /// <remarks>
        /// Only reset this if the default character interferes with your output.
        /// This is unlikely.
        ///</remarks>
        /// <remarks>
        /// The correct time to reset this value is before any other code runs.
        /// 
        /// Do not reset this if output strings are being formatted in another thread.
        /// Do not store strings containing control characters, and then reset this, as
        /// the stored string will have been formatted with the old value and will not
        /// render correctly.
        ///</remarks>
        public static char ControlSequenceIntroducer
        {
            get { return _controlSequenceIntroducer; }
            set
            {
                _controlSequenceIntroducer = value;
                RaiseControlSequenceUpdate();
            }
        }

        /// <summary>
        /// Defines the character that terminates a control sequence in console output.
        /// </summary>
        /// <remarks>
        /// Only reset this if the default character interferes with your output.
        /// This is unlikely.
        ///</remarks>
        /// <remarks>
        /// The correct time to reset this value if before any other code runs.
        /// 
        /// Do not reset this if output strings are being formatted in another thread.
        /// Do not store strings containing control characters, and then reset this, as
        /// the stored string will have been formatted with the old value and will not
        /// render correctly.
        ///</remarks>
        public static char ControlSequenceTerminator
        {
            get { return _controlSequenceTerminator; }
            set
            {
                _controlSequenceTerminator = value;
                RaiseControlSequenceUpdate();
            }
        }

        /// <summary>
        /// Convert a colour code constant into the actual console colour value.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static ConsoleColor ConsoleColourFromCode(string code)
        {
            switch (code)
            {
                case Black:
                    return ConsoleColor.Black;
                case DarkBlue:
                    return ConsoleColor.DarkBlue;
                case DarkGreen:
                    return ConsoleColor.DarkGreen;
                case DarkCyan:
                    return ConsoleColor.DarkCyan;
                case DarkRed:
                    return ConsoleColor.DarkRed;
                case DarkMagenta:
                    return ConsoleColor.DarkMagenta;
                case DarkYellow:
                    return ConsoleColor.DarkYellow;
                case Gray:
                    return ConsoleColor.Gray;
                case DarkGray:
                    return ConsoleColor.DarkGray;
                case Blue:
                    return ConsoleColor.Blue;
                case Green:
                    return ConsoleColor.Green;
                case Cyan:
                    return ConsoleColor.Cyan;
                case Red:
                    return ConsoleColor.Red;
                case Magenta:
                    return ConsoleColor.Magenta;
                case Yellow:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.White;
            }
        }

        /// <summary>
        /// Convert a console colour value into the corresponding colour code constant.
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        internal static string ConsoleColourToCode(ConsoleColor colour)
        {
            switch (colour)
            {
                case ConsoleColor.Black:
                    return Black;
                case ConsoleColor.DarkBlue:
                    return DarkBlue;
                case ConsoleColor.DarkGreen:
                    return DarkGreen;
                case ConsoleColor.DarkCyan:
                    return DarkCyan;
                case ConsoleColor.DarkRed:
                    return DarkRed;
                case ConsoleColor.DarkMagenta:
                    return DarkMagenta;
                case ConsoleColor.DarkYellow:
                    return DarkYellow;
                case ConsoleColor.Gray:
                    return Gray;
                case ConsoleColor.DarkGray:
                    return DarkGray;
                case ConsoleColor.Blue:
                    return Blue;
                case ConsoleColor.Green:
                    return Green;
                case ConsoleColor.Cyan:
                    return Cyan;
                case ConsoleColor.Red:
                    return Red;
                case ConsoleColor.Magenta:
                    return Magenta;
                case ConsoleColor.Yellow:
                    return Yellow;
                default:
                    return White;
            }
        }

        /// <summary>
        /// Set the default cpnfiguration values.
        /// </summary>
        static AdapterConfiguration()
        {
            ControlSequenceIntroducer = '\uE000';
            ControlSequenceTerminator = '\uE001';
        }
    }
}