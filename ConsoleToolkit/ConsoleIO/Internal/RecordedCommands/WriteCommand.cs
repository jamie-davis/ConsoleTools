namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal class WriteCommand : SimpleTextCommandBase, IRecordedCommand
    {
        public WriteCommand(string data) : base(data)
        {
        }

        public void Replay(ReplayBuffer buffer)
        {
            buffer.Write(ColourSplitText);
        }
    }
}
