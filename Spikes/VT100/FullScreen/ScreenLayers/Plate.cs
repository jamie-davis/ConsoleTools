using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VT100.FullScreen.ControlBehaviour;

namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// The plate is a virtual representation of the display. Data rendered to the plate is remembered but not
    /// displayed directly to the console. Once plates have been rendered they can be stacked. Areas of each
    /// plate that have not been rendered to are treated as transparent and the content of plates earlier in the
    /// stack will show through. Imagine the plates were infinitely thin plates of glass and rendering paints an opaque
    /// representation of content on the glass. The idea is that the screen is constructed from placing the plates in
    /// a stack and then looking down from above.
    /// <remarks>The control with focus is not rendered to it's original plate but to a "top" plate to ensure it's fully
    /// visible. For example, if the cursor is in a text box, the user will be able to see the whole text box regardless
    /// of whether is is actually obscured by the content of a plate higher in the stack. The idea is that the user
    /// should always be able to see the cursor and the content they are editing. To prevent this from creating a
    /// confusing visual the UI should be constructed such that any controls that can get focus are not covered by
    /// content from other plates.</remarks> 
    /// </summary>
    internal class Plate
    {
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private DisplayFormat[] _format;
        private char[] _content;
        private int _width;

        public Plate(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            Clear();
        }

        public int Width => _windowWidth;
        public int Height => _windowHeight;

        private void Clear()
        {
            var arraySize = _windowHeight * _windowWidth;
            _format = new DisplayFormat[arraySize];
            _content = new char[arraySize];
        }

        //formatting like underline
        public void WriteText(int x, int y, string text, DisplayFormat format = default)
        {
            var offset = (y * _windowWidth) + x;
            Array.Copy(text.ToCharArray(), 0, _content, offset, text.Length);
            if (!format.IsDefault())
                format.Apply(_format, offset, text.Length);
        }

        /// <summary>
        /// For debugging purposes, dump out the content of the plate 
        /// </summary>
        /// <returns>A string representing the plate</returns>
        internal string Dump(DumpType dumpType = DumpType.Text)
        {
            const string numberString = "0123456789";

            var sb = new StringBuilder();

            var numWidth = (_windowHeight - 1).ToString().Length;

            void AppendNumberLine() 
            {
                var padString = new string(' ', numWidth);
                sb.Append($"{padString}+");
                var hzNumbers = _windowWidth;
                while (hzNumbers > numberString.Length)
                {
                    sb.Append(numberString);
                    hzNumbers -= numberString.Length;
                }
                sb.AppendLine($"{numberString.Substring(0, hzNumbers)}+");
            }

            char GetContentChar(int ix)
            {
                 Dictionary<VtColour, char> ColourChars = new Dictionary<VtColour, char>
                {
                    {VtColour.NoColourChange,' '},
                    {VtColour.Black,'0'},
                    {VtColour.Red,'R'},
                    {VtColour.Green,'G'},
                    {VtColour.Yellow,'Y'},
                    {VtColour.Blue,'B'},
                    {VtColour.Magenta,'M'},
                    {VtColour.Cyan,'C'},
                    {VtColour.White, 'W'},
                    {VtColour.BrightBlack, 'g'},
                    {VtColour.BrightRed,'r'},
                    {VtColour.BrightGreen,'g'},
                    {VtColour.BrightYellow,'y'},
                    {VtColour.BrightBlue,'b'},
                    {VtColour.BrightMagenta,'m'},
                    {VtColour.BrightCyan, 'c'},
                    {VtColour.BrightWhite, 'w'},
                };
                
                switch (dumpType)
                {
                    case DumpType.Text:
                        return _content[ix] == '\0' ? ' ' : _content[ix];
                    
                    case DumpType.Colour:
                        var element = _format[ix].Colour;
                        return ColourChars[element];
                        
                    case DumpType.Formatting:
                        return '#';

                    default:
                        throw new ArgumentOutOfRangeException(nameof(dumpType), dumpType, null);
                }
                
            }

            string MakeDumpLine(int line)
            {
                var lineStart = _windowWidth * line;
                return string.Concat(Enumerable.Range(lineStart, _windowWidth).Select(ix => GetContentChar(ix)));
            }
            
            AppendNumberLine();
            
            for (var line = 0; line < _windowHeight; line++)
            {
                var lineNumText = line.ToString();
                lineNumText = new string(' ', numWidth - lineNumText.Length) + lineNumText;
                sb.AppendLine($"{lineNumText}|{MakeDumpLine(line)}|");
            }
            AppendNumberLine();

            return sb.ToString();
        }

    }

    internal enum DumpType
    {
        Text,
        Colour,
        Formatting
    }
}