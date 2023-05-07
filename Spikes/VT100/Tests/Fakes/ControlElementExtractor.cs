using System;
using System.Collections.Generic;
using System.Linq;
using VT100.Attributes;
using VT100.FullScreen;
using VT100.Utilities.ReadConsole;

namespace VT100.Tests.Fakes
{
    /// <summary>
    /// Create a fake control elements for use in unit tests.
    /// </summary>
    internal static class ControlElementExtractor
    {
        public static List<ControlElement> ListFromString(string text)
        {
            return text.Select(c => new ControlElement(MakeConsoleKeyInfo(c)))
                .ToList();
        }

        private static ConsoleKeyInfo MakeConsoleKeyInfo(char c)
        {
            var shiftKey = c >= 'A' && c <= 'Z';
            return new ConsoleKeyInfo(c, ConsoleKeyConverter.FromChar(c) ?? 0, shiftKey, false, false);
        }

        private static List<ControlElement> ToControlElementsList(string text)
        {
            return text
                .Select(c => new ControlElement(new ConsoleKeyInfo(c, ConsoleKeyConverter.FromChar(c) ?? new ConsoleKey(), false, false, false)))
                .ToList();
        }
    }
}