using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal class SimpleTextCommandBase
    {
        protected string _data;

        private int _tabLength = -1;
        private int _firstWordLength;
        private int _longestWordLength; 
        private List<ColourControlItem> _colourSplitText;
        private List<SplitWord> _splitWords;

        public SimpleTextCommandBase(string data)
        {
            _data = data;
            _colourSplitText = ColourControlSplitter.Split(data);
        }

        protected List<ColourControlItem> ColourSplitText { get { return _colourSplitText; } }

        private void SplitText(int tabLength)
        {
            if (tabLength >= 0 && tabLength == _tabLength && _colourSplitText != null)
                return;

            _tabLength = tabLength;
            _splitWords = WordSplitter.SplitToList(_colourSplitText, tabLength);
            var firstWord = _splitWords.FirstOrDefault();

            _firstWordLength = firstWord == null ? 0 : firstWord.Length;
            _longestWordLength = _splitWords.Any() ? _splitWords.Max(w => w.Length) : 0;
        }

        public int GetFirstWordLength(int tabLength)
        {
            if (tabLength == _tabLength && tabLength >= 0)
                return _firstWordLength;

            SplitText(tabLength);
            return _firstWordLength;
        }

        public int GetLongestWordLength(int tabLength)
        {
            if (tabLength == _tabLength && tabLength >= 0)
                return _longestWordLength;

            SplitText(tabLength);
            return _longestWordLength;
        }
    }
}