namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal class WrapCommand : SimpleTextCommandBase, IRecordedCommand
    {
        public WrapCommand(string data) : base(data)
        {
        }

        public void Replay(ReplayBuffer buffer)
        {
            buffer.Wrap(_data);
        }
    }
}