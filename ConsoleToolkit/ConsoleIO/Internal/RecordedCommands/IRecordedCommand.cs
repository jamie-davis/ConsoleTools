namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    interface IRecordedCommand
    {
        void Replay(ReplayBuffer buffer);
        int GetFirstWordLength(int tabLength);
        int GetLongestWordLength(int tabLength);
    }
}
