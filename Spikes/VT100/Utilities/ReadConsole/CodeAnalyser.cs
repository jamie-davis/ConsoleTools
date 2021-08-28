using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace VT100.Utilities.ReadConsole
{
    internal static class CodeAnalyser
    {
        private static Dictionary<ResolvedCode, Func<List<ControlElement>, bool>> _csiResolvers =
            new Dictionary<ResolvedCode, Func<List<ControlElement>, bool>>
            {
                { ResolvedCode.CursorBackwards, s => Match(s, 2, 'D') },
                { ResolvedCode.CursorForward, s => Match(s, 2, 'C') },
                { ResolvedCode.CursorUp, s => Match(s, 2, 'A') },
                { ResolvedCode.CursorDown, s => Match(s, 2, 'B') },
                { ResolvedCode.PF5, s => Match(s, 2, '1', '5', '~') },
                { ResolvedCode.PF6, s => Match(s, 2, '1', '7', '~') },
                { ResolvedCode.PF7, s => Match(s, 2, '1', '8', '~') },
                { ResolvedCode.PF8, s => Match(s, 2, '1', '9', '~') },
                { ResolvedCode.PF9, s => Match(s, 2, '2', '0', '~') },
                { ResolvedCode.PF10, s => Match(s, 2,'2', '1', '~') },
                { ResolvedCode.PF11, s => Match(s, 2,'2', '3', '~') },
                { ResolvedCode.PF12, s => Match(s, 2,'2', '4', '~') },
                { ResolvedCode.PF13, s => Match(s, 2,'2', '5', '~') },
                { ResolvedCode.PF14, s => Match(s, 2,'2', '6', '~') },
                { ResolvedCode.PF15, s => Match(s, 2,'2', '8', '~') },
                { ResolvedCode.PF16, s => Match(s, 2,'2', '9', '~') },
                { ResolvedCode.PF17, s => Match(s, 2,'3', '1', '~') },
                { ResolvedCode.PF18, s => Match(s, 2,'3', '2', '~') },
                { ResolvedCode.PF19, s => Match(s, 2,'3', '3', '~') },
                { ResolvedCode.PF20, s => Match(s, 2,'3', '4', '~') },
                { ResolvedCode.Home, s => Match(s, 2,'H') },
                { ResolvedCode.End, s => Match(s, 2,'F') },
            };        
        private static Dictionary<ResolvedCode, Func<List<ControlElement>, bool>> _ss3Resolvers =
            new Dictionary<ResolvedCode, Func<List<ControlElement>, bool>>
            {
                { ResolvedCode.PF1, s => Match(s, 2, 'P') },
                { ResolvedCode.PF2, s => Match(s, 2, 'Q') },
                { ResolvedCode.PF3, s => Match(s, 2, 'R') },
                { ResolvedCode.PF4, s => Match(s, 2, 'S') },
                { ResolvedCode.Home, s => Match(s, 2, 'H') },
                { ResolvedCode.End, s => Match(s, 2, 'F') },
            };

        internal static (ResolvedCode Code, IEnumerable<string> Parameters) 
            Analyse(IEnumerable<ControlElement> elements, AnsiCodeType codeType)
        {
            var (seq, parameters) = CodeSequenceParameterExtractor.Extract(elements, codeType);
            if (codeType == AnsiCodeType.CSI)
            {
                var match = _csiResolvers.FirstOrDefault(r => r.Value(seq));
                if (match.Key != ResolvedCode.NotRecognised)
                    return (match.Key, parameters);
            }
            else if (codeType == AnsiCodeType.SS3)
            {
                var match = _ss3Resolvers.FirstOrDefault(r => r.Value(seq));
                if (match.Key != ResolvedCode.NotRecognised)
                    return (match.Key, parameters);
            }

            return (ResolvedCode.NotRecognised, new List<string>());
        }

        private static bool IsForward(List<ControlElement> seq, AnsiCodeType codeType)
        {
            if (codeType != AnsiCodeType.CSI || seq.Count < 3) return false;

            return Match(seq, 2, 'C');
        }

        private static bool Match(List<ControlElement> seq, int start, params char[] chars)
        {
            return seq.Skip(start).Select(e => e.Key.KeyChar).SequenceEqual(chars);
        }

    }
}
