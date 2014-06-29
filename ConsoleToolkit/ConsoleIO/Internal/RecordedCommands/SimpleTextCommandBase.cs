using System;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal class SimpleTextCommandBase
    {
        protected string _data;

        public SimpleTextCommandBase(string data)
        {
            _data = data;
        }

        public int GetFirstWordLength(int tabLength)
        {
            var splitWords = WordSplitter.Split(_data, tabLength);
            if (splitWords.Any())
                return splitWords.First().Length;
            return 0;
        }

        public int GetLongestWordLength(int tabLength)
        {
            var splitWords = WordSplitter.Split(_data, tabLength);
            if (splitWords.Any())
                return splitWords.Max(w => w.Length);
            return 0;
        }
    }
}