using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace VT100.Utilities.ReadConsole
{
    internal class CodeAnalyser
    {
        private readonly CodeAnalyserSettings _settings;

        private (ResolvedCode Code, Func<List<ControlElement>, bool> Fn) [] _csiResolvers =
            {
                ( ResolvedCode.CursorBackwards, s => Match(s, 2, 'D') ),
                ( ResolvedCode.CursorForward, s => Match(s, 2, 'C') ),
                ( ResolvedCode.CursorUp, s => Match(s, 2, 'A') ),
                ( ResolvedCode.CursorDown, s => Match(s, 2, 'B') ),
                ( ResolvedCode.PF1,	s => Match(s, 2, '1', '1', '~') ),
                ( ResolvedCode.PF1,	s => Match(s, 2, '1', 'P') ),
                ( ResolvedCode.PF2,	s => Match(s, 2, '1', '2', '~') ),
                ( ResolvedCode.PF2,	s => Match(s, 2, '1', 'Q') ),
                ( ResolvedCode.PF3,	s => Match(s, 2, '1', '3', '~') ),
                ( ResolvedCode.PF3,	s => Match(s, 2, '1', 'R') ),
                ( ResolvedCode.PF4,	s => Match(s, 2, '1', '4', '~') ),
                ( ResolvedCode.PF4,	s => Match(s, 2, '1', 'S') ),
                ( ResolvedCode.PF5, s => Match(s, 2, '1', '5', '~') ),
                ( ResolvedCode.PF6, s => Match(s, 2, '1', '7', '~') ),
                ( ResolvedCode.PF7, s => Match(s, 2, '1', '8', '~') ),
                ( ResolvedCode.PF8, s => Match(s, 2, '1', '9', '~') ),
                ( ResolvedCode.PF9, s => Match(s, 2, '2', '0', '~') ),
                ( ResolvedCode.PF10, s => Match(s, 2,'2', '1', '~') ),
                ( ResolvedCode.PF11, s => Match(s, 2,'2', '3', '~') ),
                ( ResolvedCode.PF12, s => Match(s, 2,'2', '4', '~') ),
                ( ResolvedCode.PF13, s => Match(s, 2,'2', '5', '~') ),
                ( ResolvedCode.PF14, s => Match(s, 2,'2', '6', '~') ),
                ( ResolvedCode.PF15, s => Match(s, 2,'2', '8', '~') ),
                ( ResolvedCode.PF16, s => Match(s, 2,'2', '9', '~') ),
                ( ResolvedCode.PF17, s => Match(s, 2,'3', '1', '~') ),
                ( ResolvedCode.PF18, s => Match(s, 2,'3', '2', '~') ),
                ( ResolvedCode.PF19, s => Match(s, 2,'3', '3', '~') ),
                ( ResolvedCode.PF20, s => Match(s, 2,'3', '4', '~') ),
                ( ResolvedCode.Home, s => Match(s, 2,'H') ),
                ( ResolvedCode.End, s => Match(s, 2,'F') ),
                ( ResolvedCode.Delete, s => Match(s, 2,'3', '~') ),
                ( ResolvedCode.Insert, s => Match(s, 2,'2', '~') ),
                ( ResolvedCode.PageDown, s => Match(s, 2,'6', '~') ),
                ( ResolvedCode.PageUp, s => Match(s, 2,'5', '~') ),
                ( ResolvedCode.Begin, s => Match(s, 2,'E') ),
                ( ResolvedCode.CPR, s => Match(s, 2, 'R') ),
                ( ResolvedCode.CBT, s => Match(s, 2, 'Z') ),
            };

        private (ResolvedCode Code, Func<List<ControlElement>, bool> Fn) [] _ss3Resolvers =
            {
                ( ResolvedCode.PF1, s => Match(s, 2, 'P') ),
                ( ResolvedCode.PF2, s => Match(s, 2, 'Q') ),
                ( ResolvedCode.PF3, s => Match(s, 2, 'R') ),
                ( ResolvedCode.PF4, s => Match(s, 2, 'S') ),
                ( ResolvedCode.Home, s => Match(s, 2, 'H') ),
                ( ResolvedCode.End, s => Match(s, 2, 'F') ),
                ( ResolvedCode.Space, s => Match(s, 2, ' ') ),
                ( ResolvedCode.Tab, s => Match(s, 2, 'I') ),
                ( ResolvedCode.CR, s => Match(s, 2, 'M') ),
            };
        private (ResolvedCode Code, Func<List<ControlElement>, bool> Fn) [] _charResolvers =
            {
                ( ResolvedCode.Tab, s => Match(s, 0, '\x9') ),
                ( ResolvedCode.CR, s => Match(s, 0, '\r') ),
                ( ResolvedCode.Backspace, s => Match(s, 0, '\x7f') ),
                ( ResolvedCode.Escape, s => Match(s, 0, '\x1b') ),
            };

        public CodeAnalyser(CodeAnalyserSettings settings = 0)
        {
            _settings = settings;
        }

        internal (ResolvedCode Code, IEnumerable<string> Parameters) 
            Analyse(IEnumerable<ControlElement> elements, AnsiCodeType codeType)
        {
            var (seq, parameters) = CodeSequenceParameterExtractor.Extract(elements, codeType);
            switch (codeType)
            {
                case AnsiCodeType.CSI:
                {
                    var match = _csiResolvers.FirstOrDefault(r => r.Fn(seq));
                    if (match.Code != ResolvedCode.NotRecognised)
                    {
                        if (OverrideToPF3(match, parameters))
                        {
                            parameters.RemoveAt(0);
                            return (ResolvedCode.PF3, parameters);
                        }

                        return (match.Code, parameters);
                    }

                    break;
                }
                case AnsiCodeType.SS3:
                {
                    var match = _ss3Resolvers.FirstOrDefault(r => r.Fn(seq));
                    if (match.Code != ResolvedCode.NotRecognised)
                        return (match.Code, parameters);
                    break;
                }
                case AnsiCodeType.None:
                {
                    var match = _charResolvers.FirstOrDefault(r => r.Fn(seq));
                    if (match.Code != ResolvedCode.NotRecognised)
                        return (match.Code, parameters);
                    break;
                }
            }

            return (ResolvedCode.NotRecognised, new List<string>());
        }

        private bool OverrideToPF3((ResolvedCode Code, Func<List<ControlElement>, bool> Fn) match, List<string> parameters)
        {
            var overrideEnabled = (_settings & CodeAnalyserSettings.PreferPF3Modifiers) > 0;
            return overrideEnabled 
                   && match.Code == ResolvedCode.CPR
                   && parameters.Count >= 1
                   && parameters[0] == "1";
        }

        private static bool Match(List<ControlElement> seq, int start, params char[] chars)
        {
            return seq.Skip(start).Select(e => e.KeyChar).SequenceEqual(chars);
        }

    }
}
