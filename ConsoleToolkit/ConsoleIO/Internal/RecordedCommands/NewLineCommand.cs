namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal class NewLineCommand : IRecordedCommand
    {
        public void Replay(ReplayBuffer buffer)
        {
            buffer.NewLine();
        }

        public int GetFirstWordLength(int tabLength)
        {
            return 0;
        }

        public int GetLongestWordLength(int tabLength)
        {
            return 0;
        }
    }
}