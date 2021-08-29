using System.Collections.Generic;
using System.Linq;

namespace VT100.Utilities.ReadConsole
{
    internal class ControlSequence
    {
        public ControlSequence(IEnumerable<ControlElement> seq, AnsiCodeType codeType, CodeAnalyserSettings settings)
        {
            CodeType = codeType;
            Items = seq.ToList();
            var analyser = new CodeAnalyser(settings);
            var resolved = analyser.Analyse(Items, codeType);
            ResolvedCode = resolved.Code;
            Parameters = resolved.Parameters;
        }

        public AnsiCodeType CodeType { get; }
        public IReadOnlyList<ControlElement> Items { get; }
        public ResolvedCode ResolvedCode { get; }
        public IEnumerable<string> Parameters { get; set; }
    }
}
