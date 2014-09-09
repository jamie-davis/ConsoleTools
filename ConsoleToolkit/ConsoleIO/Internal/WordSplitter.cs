using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class WordSplitter
    {
        private class SplitImpl
        {
            private readonly int _tabLength;
            public readonly List<SplitWord> Words = new List<SplitWord>();
            private readonly List<ColourControlItem.ControlInstruction> _cachedPrefixInstructions = new List<ColourControlItem.ControlInstruction>();
            private SplitWord _current;

            public SplitImpl(string data, int tabLength)
            {
                _tabLength = tabLength;
                var items = ColourControlSplitter.Split(data);
                _current = null;
                InitFromColourControlItems(items);
            }

            public SplitImpl(IEnumerable<ColourControlItem> data, int tabLength)
            {
                _tabLength = tabLength;
                _current = null;
                InitFromColourControlItems(data);
            }

            private void InitFromColourControlItems(IEnumerable<ColourControlItem> items)
            {
                foreach (var colourControlItem in items)
                {
                    ProcessColourControlItem(colourControlItem);
                }
                StoreCurrentIfPresent();

                if (_cachedPrefixInstructions.Any())
                {
                    var lastWord = new SplitWord(0, 0, string.Empty);
                    lastWord.AddPrefixInstructions(_cachedPrefixInstructions);
                    Words.Add(lastWord);
                }
            }

            private void ProcessColourControlItem(ColourControlItem colourControlItem)
            {
                if (colourControlItem.PrefixAffinity && string.IsNullOrEmpty(colourControlItem.Text))
                {
                    _cachedPrefixInstructions.AddRange(colourControlItem.Instructions);

                    StoreCurrentIfPresent();
                    return;
                }

                if (!string.IsNullOrEmpty(colourControlItem.Text))
                {
                    StoreCurrentIfPresent();

                    var splitWords = SplitWords(colourControlItem.Text, _tabLength);
                    if (splitWords.Any())
                    {
                        splitWords.First().AddPrefixInstructions(_cachedPrefixInstructions);
                        _cachedPrefixInstructions.Clear();
                    }

                    if (splitWords.Length > 1)
                        Words.AddRange(splitWords.Take(splitWords.Length - 1));

                    _current = splitWords[splitWords.Length - 1];
                }
                else if (InstructionIsNewLineOnly(colourControlItem))
                {
                    StoreCurrentIfPresent();
                    _current = new SplitWord(0, 0, string.Empty);                    
                    _current.AddPrefixInstructions(_cachedPrefixInstructions);
                    _current.AddSuffixInstructions(colourControlItem.Instructions);
                    _cachedPrefixInstructions.Clear();
                }
                else
                    AddSuffixInstructionsToCurrent(colourControlItem.Instructions);
            }

            private bool InstructionIsNewLineOnly(ColourControlItem item)
            {
                return item.Instructions.All(i => i.Code == ColourControlItem.ControlCode.NewLine);
            }

            private void AddSuffixInstructionsToCurrent(List<ColourControlItem.ControlInstruction> instructions)
            {
                if (_current == null)
                    _current = new SplitWord(0, 0, string.Empty);

                _current.AddSuffixInstructions(instructions);
            }

            private void StoreCurrentIfPresent()
            {
                if (_current != null)
                {
                    Words.Add(_current);
                    _current = null;
                }
            }
        }

        private static readonly string WordTermChars = ",.";
        private static readonly string SpaceChars = " \t\r\n";
        private static readonly char[] SplitChars = (SpaceChars + WordTermChars).ToCharArray();

        public static SplitWord[] Split(string data, int tabLength)
        {
            return new SplitImpl(data, tabLength).Words.ToArray();
        }

        public static List<SplitWord> SplitToList(string data, int tabLength)
        {
            return new SplitImpl(data, tabLength).Words;
        }

        public static IEnumerable<SplitWord> Split(IEnumerable<ColourControlItem> data, int tabLength)
        {
            return new SplitImpl(data, tabLength).Words.ToArray();
        }

        public static List<SplitWord> SplitToList(IEnumerable<ColourControlItem> data, int tabLength)
        {
            return new SplitImpl(data, tabLength).Words;
        }

        private static SplitWord[] SplitWords(string data, int tabLength)
        {
            var words = new List<SplitWord>();
            var dataPos = 0;
            while (dataPos < data.Length)
            {
                var nextEnd = data.IndexOfAny(SplitChars, dataPos);
                if (nextEnd < 0)
                {
                    var length = data.Length - dataPos;
                    words.Add(new SplitWord(length, 0, data.Substring(dataPos, length)));
                    break;
                }

                if (nextEnd < data.Length && WordTermChars.Contains(data[nextEnd]))
                    ++nextEnd;

                var wordLen = nextEnd - dataPos;
                var spaces = 0;
                while (nextEnd < data.Length && SpaceChars.Contains(data[nextEnd]))
                {
                    var nextChar = data[nextEnd];
                    if (nextChar == '\r' || nextChar == '\n')
                    {
                        ++nextEnd;
                        if (nextChar == '\r' && nextEnd < data.Length && data[nextEnd] == '\n')
                        {
                            ++nextEnd;
                        }
                        break;
                    }
                    spaces += nextChar == ' ' ? 1 : tabLength;
                    ++nextEnd;
                }

                var wordValue = data.Substring(dataPos, wordLen);
                words.Add(new SplitWord(GetVisibleLength(wordValue), spaces, wordValue));
                dataPos = nextEnd;
            }
            return words.ToArray();
        }

        private static int GetVisibleLength(string wordValue)
        {
            var components = ColourControlSplitter.Split(wordValue);
            return components.Sum(arg => arg.Text != null ? arg.Text.Length : 0);
        }
    }
}