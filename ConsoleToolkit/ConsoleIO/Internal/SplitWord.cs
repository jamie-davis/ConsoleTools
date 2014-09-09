using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class SplitWord
    {
        private readonly List<ColourControlItem.ControlInstruction> _prefixInstructions = new List<ColourControlItem.ControlInstruction>();
        private readonly List<ColourControlItem.ControlInstruction> _suffixInstructions = new List<ColourControlItem.ControlInstruction>();
        private Lazy<bool> _terminatesLine;
        public int Length { get; private set; }
        public int TrailingSpaces { get; private set; }
        public string WordValue { get; private set; }

        public IEnumerable<ColourControlItem.ControlInstruction> PrefixInstructions { get { return _prefixInstructions; } } 
        public IEnumerable<ColourControlItem.ControlInstruction> SuffixInstructions { get { return _suffixInstructions; } } 

        public SplitWord(int length, int trailingSpaces, string wordValue)
        {
            Length = length;
            TrailingSpaces = trailingSpaces;
            WordValue = wordValue;
            _terminatesLine = new Lazy<bool>(() => _suffixInstructions.Any(i => i.Code == ColourControlItem.ControlCode.NewLine));
        }

        public string GetWordValue()
        {
            return ReconstituteInstructions(true) + WordValue;
        }

        public string GetTrailingSpaces(int maxWidth, out int spacesAdded)
        {
            spacesAdded = Math.Min(maxWidth, TrailingSpaces);
            return new string(' ', spacesAdded) + ReconstituteInstructions(false);
        }

        private string ReconstituteInstructions(bool prefix)
        {
            string affinity;
            List<ColourControlItem.ControlInstruction> collection;
            if (prefix)
            {
                collection = _prefixInstructions;
                affinity = AdapterConfiguration.PrefixAffinity;
            }
            else
            {
                collection = _suffixInstructions;
                affinity = AdapterConfiguration.SuffixAffinity;
            }

            if (!collection.Any()) return string.Empty;

            return ColourInstructionRebuilder.Rebuild(affinity, collection);
        }

        public bool TerminatesLine()
        {
            return _terminatesLine.Value;
        }

        public void AddPrefixInstructions(IEnumerable<ColourControlItem.ControlInstruction> instructions)
        {
            _prefixInstructions.AddRange(instructions);    
        }

        public void AddSuffixInstructions(IEnumerable<ColourControlItem.ControlInstruction> instructions)
        {
            _suffixInstructions.AddRange(instructions);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("SplitWord:");
            if (_prefixInstructions != null && _prefixInstructions.Any())
            {
                sb.Append("{>");
                foreach (var prefix in _prefixInstructions)
                    sb.Append(prefix);
                sb.Append("}");
            }

            if (!string.IsNullOrEmpty(WordValue))
            {
                sb.Append("\"");
                sb.Append(WordValue);
                sb.Append("\"");
            }

            if (_suffixInstructions != null && _suffixInstructions.Any())
            {
                sb.Append("{<");
                foreach (var suffix in _suffixInstructions)
                    sb.Append(suffix);
                sb.Append("}");
            }

            return sb.ToString();
        }
    }
}