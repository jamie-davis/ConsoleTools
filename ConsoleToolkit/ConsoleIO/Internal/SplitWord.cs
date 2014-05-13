namespace ConsoleToolkit.ConsoleIO.Internal
{
    public class SplitWord
    {
        public int Length { get; private set; }
        public int TrailingSpaces { get; private set; }
        public bool TerminatesLine { get; private set; }
        public string WordValue { get; private set; }

        public SplitWord(int length, int trailingSpaces, bool terminatesLine, string wordValue)
        {
            Length = length;
            TrailingSpaces = trailingSpaces;
            TerminatesLine = terminatesLine;
            WordValue = wordValue;
        }
    }
}