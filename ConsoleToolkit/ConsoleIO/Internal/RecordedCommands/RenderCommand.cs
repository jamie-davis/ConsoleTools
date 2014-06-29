using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal class RenderCommand : IRecordedCommand
    {
        private readonly IConsoleRenderer _data;

        public RenderCommand(IConsoleRenderer data)
        {
            _data = data;
        }

        public void Replay(ReplayBuffer buffer)
        {
            if (buffer.CursorLeft > 0)
                buffer.NewLine();

            int wrappedLines;
            var lines = _data.Render(buffer.Width, out wrappedLines).ToList();
            if (lines.Count == 0) return;
            
            foreach (var line in lines.Where((l, i) => i < lines.Count - 1))
            {
                buffer.Write(line);
                buffer.NewLine();
            }
            buffer.Write(lines.Last());
        }

        public int GetFirstWordLength(int tabLength)
        {
            throw new System.NotImplementedException();
        }

        public int GetLongestWordLength(int tabLength)
        {
            throw new System.NotImplementedException();
        }
    }
}