using System;
using System.Collections.Generic;
using System.Linq;
using Vt100.FullScreen;
using Vt100.FullScreen.Controls;
using VT100.Utilities.ReadConsole;

namespace VT100.Tests.TestUtilities
{
    internal static class ControlInputFeeder
    {
        private static Dictionary<ResolvedCode, ControlSequence> _sequences =
            new Dictionary<ResolvedCode, ControlSequence>
            {
                { ResolvedCode.CursorForward, MakeControlSequence(AnsiCodeType.CSI, '\x1b', '[', 'C') },
                { ResolvedCode.CursorBackwards, MakeControlSequence(AnsiCodeType.CSI, '\x1b', '[', 'D') },
                { ResolvedCode.End, MakeControlSequence(AnsiCodeType.CSI, '\x1b', 'O', 'F') },
                { ResolvedCode.Home, MakeControlSequence(AnsiCodeType.CSI, '\x1b', 'O', 'H') },
                { ResolvedCode.Tab, MakeControlSequence(AnsiCodeType.None, '\t')},
                { ResolvedCode.Backspace, MakeControlSequence(AnsiCodeType.None, '\x7f')},
                { ResolvedCode.CR, MakeControlSequence(AnsiCodeType.None, '\r')},
                { ResolvedCode.NotRecognised, MakeControlSequence(AnsiCodeType.None, 'X')},
                { ResolvedCode.Escape, MakeControlSequence(AnsiCodeType.None, '\x1b')},
            };

        private static ControlSequence MakeControlSequence(AnsiCodeType codeType, params char[] chars)
        {
            var elements = chars
                .Select(c => new ControlElement(new ConsoleKeyInfo(c, (ConsoleKey)((int)c), false, false, false)))
                .ToList();
            return new ControlSequence(elements, codeType, 0);
        }

        public static void Process(ILayoutControl control, object[] data, Action<string> postInputProcessing = null)
        {
            foreach (var keyRequest in data)
            {
                if (keyRequest is char charRequest)
                    control.Accept(MakeControlSequence(AnsiCodeType.None, charRequest));
                else if (keyRequest is ResolvedCode code && _sequences.TryGetValue(code, out var sequence))
                    control.Accept(sequence);
                else
                    continue;

                if (postInputProcessing != null)
                    postInputProcessing(keyRequest.ToString());
            }
        }
    }
}