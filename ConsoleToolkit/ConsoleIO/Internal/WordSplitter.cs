using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class WordSplitter
    {
        private static readonly string WordTermChars = ",.";
        private static readonly string SpaceChars = " \t\r\n";
        private static readonly char[] SplitChars = (SpaceChars + WordTermChars).ToCharArray();

        public static SplitWord[] Split(string data, int tabLength)
        {
            var words = new List<SplitWord>();
            var dataPos = 0;
            while (dataPos < data.Length)
            {
                var nextEnd = data.IndexOfAny(SplitChars, dataPos);
                if (nextEnd < 0)
                {
                    var length = data.Length - dataPos;
                    words.Add(new SplitWord(length, 0, false, data.Substring(dataPos, length)));
                    break;
                }

                if (nextEnd < data.Length && WordTermChars.Contains(data[nextEnd]))
                    ++nextEnd;

                var wordLen = nextEnd - dataPos;
                var spaces = 0;
                var terminatesLine = false;
                while (nextEnd < data.Length && SpaceChars.Contains(data[nextEnd]))
                {
                    var nextChar = data[nextEnd];
                    if (nextChar == '\r' || nextChar == '\n')
                    {
                        ++nextEnd;
                        terminatesLine = true;
                        if (nextChar == '\r' && nextEnd < data.Length && data[nextEnd] == '\n')
                        {
                            ++nextEnd;
                        }
                        break;
                    }
                    spaces += nextChar == ' ' ? 1 : tabLength;
                    ++nextEnd;
                }

                words.Add(new SplitWord(wordLen, spaces, terminatesLine, data.Substring(dataPos, wordLen)));
                dataPos = nextEnd;
            }
            return words.ToArray();
        }
    }
}