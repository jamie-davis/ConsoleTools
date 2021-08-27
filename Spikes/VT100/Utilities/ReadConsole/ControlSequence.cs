using System.Collections.Generic;
using System.Linq;

namespace VT100.Utilities.ReadConsole
{
    internal class ControlSequence
    {
        public ControlSequence(IEnumerable<ControlElement> seq, AnsiCodeType codeType)
        {
            CodeType = codeType;
            Items = seq.ToList();
            ResolvedCode = CodeAnalyser.Analyse(Items, codeType);
        }

        public AnsiCodeType CodeType { get; }
        public IReadOnlyList<ControlElement> Items { get; }
        public ResolvedCode ResolvedCode { get; }
    }
}
