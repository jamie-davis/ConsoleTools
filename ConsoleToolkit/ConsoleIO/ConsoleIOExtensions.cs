using System.Collections.Generic;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// Extension methods for customising text that will be output to the console.
    /// 
    /// These methods allow display instructions to be imbedded into strings easily.
    /// <example>
    /// <code>
    ///      var myText = "my text".Red();
    /// </code>
    /// 
    /// When <c>myText</c> is written out to the console, it will be rendered with a 
    /// red foreground colour.
    /// </example>
    /// </summary>
    public static class ConsoleIOExtensions
    {
        class ColourControl
        {
            public string ColourCode { get; private set; }
            public string SetForeground { get; private set; } 
            public string SetBackground { get; private set; }

            public ColourControl(string colourCode)
            {
                ColourCode = colourCode;
                UpdateCSI();
            }

            public void UpdateCSI()
            {
                SetForeground = AdapterConfiguration.ControlSequenceIntroducer +
                                AdapterConfiguration.PrefixAffinity + 
                                AdapterConfiguration.PushInstruction + 
                                AdapterConfiguration.SetForegroundInstruction + 
                                ColourCode +
                                AdapterConfiguration.ControlSequenceTerminator;
                SetBackground = AdapterConfiguration.ControlSequenceIntroducer +
                                AdapterConfiguration.PrefixAffinity + 
                                AdapterConfiguration.PushInstruction + 
                                AdapterConfiguration.SetBackgroundInstruction + 
                                ColourCode +
                                AdapterConfiguration.ControlSequenceTerminator;
            }
        }

        // Colour control sequence information
        private static readonly ColourControl ColourBlack = new ColourControl(AdapterConfiguration.Black);
        private static readonly ColourControl ColourDarkBlue = new ColourControl(AdapterConfiguration.DarkBlue);
        private static readonly ColourControl ColourDarkGreen = new ColourControl(AdapterConfiguration.DarkGreen);
        private static readonly ColourControl ColourDarkCyan = new ColourControl(AdapterConfiguration.DarkCyan);
        private static readonly ColourControl ColourDarkRed = new ColourControl(AdapterConfiguration.DarkRed);
        private static readonly ColourControl ColourDarkMagenta = new ColourControl(AdapterConfiguration.DarkMagenta);
        private static readonly ColourControl ColourDarkYellow = new ColourControl(AdapterConfiguration.DarkYellow);
        private static readonly ColourControl ColourGray = new ColourControl(AdapterConfiguration.Gray);
        private static readonly ColourControl ColourDarkGray = new ColourControl(AdapterConfiguration.DarkGray);
        private static readonly ColourControl ColourBlue = new ColourControl(AdapterConfiguration.Blue);
        private static readonly ColourControl ColourGreen = new ColourControl(AdapterConfiguration.Green);
        private static readonly ColourControl ColourCyan = new ColourControl(AdapterConfiguration.Cyan);
        private static readonly ColourControl ColourRed = new ColourControl(AdapterConfiguration.Red);
        private static readonly ColourControl ColourMagenta = new ColourControl(AdapterConfiguration.Magenta);
        private static readonly ColourControl ColourYellow = new ColourControl(AdapterConfiguration.Yellow);
        private static readonly ColourControl ColourWhite = new ColourControl(AdapterConfiguration.White);
        private static string Pop;

        private static object _lock = new object();

        private static Dictionary<string, string> _colourVariables = new Dictionary<string, string>(); 

        static ConsoleIOExtensions()
        {
            AdapterConfiguration.ControlSequenceUpdate += RebuildControlSequences;
            BuildControlSequences();
        }

        private static void RebuildControlSequences(object sender, AdapterConfiguration.ControlSequenceUpdateEventArgs e)
        {
            BuildControlSequences();
        }

        private static void BuildControlSequences()
        {
            lock (_lock)
            {
                MakePopInstruction();
                ColourBlack.UpdateCSI();
                ColourDarkBlue.UpdateCSI();
                ColourDarkGreen.UpdateCSI();
                ColourDarkCyan.UpdateCSI();
                ColourDarkRed.UpdateCSI();
                ColourDarkMagenta.UpdateCSI();
                ColourDarkYellow.UpdateCSI();
                ColourGray.UpdateCSI();
                ColourDarkGray.UpdateCSI();
                ColourBlue.UpdateCSI();
                ColourGreen.UpdateCSI();
                ColourCyan.UpdateCSI();
                ColourRed.UpdateCSI();
                ColourMagenta.UpdateCSI();
                ColourYellow.UpdateCSI();
                ColourWhite.UpdateCSI();
            }
        }

        private static void MakePopInstruction()
        {
            Pop = AdapterConfiguration.ControlSequenceIntroducer +
                  AdapterConfiguration.SuffixAffinity +
                  AdapterConfiguration.PopInstruction +
                  AdapterConfiguration.ControlSequenceTerminator;
        }

        // ReSharper disable InconsistentNaming
        #region Foreground setters

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a black foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string Black(this string s)
        {
            return string.Format("{0}{1}{2}", ColourBlack.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark blue colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string DarkBlue(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkBlue.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark green foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string DarkGreen(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkGreen.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark cyan colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string DarkCyan(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkCyan.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark red foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string DarkRed(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkRed.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark magenta foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string DarkMagenta(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkMagenta.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark yellow foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string DarkYellow(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkYellow.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a grey foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string Gray(this string s)
        {
            return string.Format("{0}{1}{2}", ColourGray.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark grey foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string DarkGray(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkGray.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a blue foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string Blue(this string s)
        {
            return string.Format("{0}{1}{2}", ColourBlue.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a green foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string Green(this string s)
        {
            return string.Format("{0}{1}{2}", ColourGreen.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a cyan foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string Cyan(this string s)
        {
            return string.Format("{0}{1}{2}", ColourCyan.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a red foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string Red(this string s)
        {
            return string.Format("{0}{1}{2}", ColourRed.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a magenta foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string Magenta(this string s)
        {
            return string.Format("{0}{1}{2}", ColourMagenta.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a yellow foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string Yellow(this string s)
        {
            return string.Format("{0}{1}{2}", ColourYellow.SetForeground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a white foreground colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string White(this string s)
        {
            return string.Format("{0}{1}{2}", ColourWhite.SetForeground, s, Pop);
        }

        #endregion

        
        #region Background Setters

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a black background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGBlack(this string s)
        {
            return string.Format("{0}{1}{2}", ColourBlack.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark blue background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGDarkBlue(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkBlue.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark green background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGDarkGreen(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkGreen.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark cyan background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGDarkCyan(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkCyan.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark red background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGDarkRed(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkRed.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark magenta background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGDarkMagenta(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkMagenta.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark yellow background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGDarkYellow(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkYellow.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a grey background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGGray(this string s)
        {
            return string.Format("{0}{1}{2}", ColourGray.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a dark grey background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGDarkGray(this string s)
        {
            return string.Format("{0}{1}{2}", ColourDarkGray.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a blue background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGBlue(this string s)
        {
            return string.Format("{0}{1}{2}", ColourBlue.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a green background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGGreen(this string s)
        {
            return string.Format("{0}{1}{2}", ColourGreen.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a cyan background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGCyan(this string s)
        {
            return string.Format("{0}{1}{2}", ColourCyan.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a red background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGRed(this string s)
        {
            return string.Format("{0}{1}{2}", ColourRed.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a magenta background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGMagenta(this string s)
        {
            return string.Format("{0}{1}{2}", ColourMagenta.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a yellow background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGYellow(this string s)
        {
            return string.Format("{0}{1}{2}", ColourYellow.SetBackground, s, Pop);
        }

        /// <summary>
        /// Add instructions to a string that will cause it to be rendered with a white background colour.
        /// <remarks>
        /// Note that the returned string will be extended with control characters that will be visible if
        /// the string is displayed directly. After using this method, the string will only be suitable for
        /// output using a <see cref="ConsoleAdapter"/> which can interpret the instructions.
        /// </remarks><remarks>
        /// The actual instruction codes used by the ConsoleToolkit are not documented, and are subject to change
        /// in <em>any</em> release. You must ensure that all string manipulation has been done before the colour
        /// instruction codes are added.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to be altered.</param>
        /// <returns>The string with added instruction codes.</returns>
        public static string BGWhite(this string s)
        {
            return string.Format("{0}{1}{2}", ColourWhite.SetBackground, s, Pop);
        }

        #endregion
    }
}