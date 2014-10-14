using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    public class RecordingConsoleAdapter : IConsoleOperations, IConsoleRenderer
    {
        private List<IRecordedCommand> _steps = new List<IRecordedCommand>();

        public void WriteLine(string format, params object[] arg)
        {
            Write(format, arg);
            WriteLine();
        }

        public void Write(string format, params object[] arg)
        {
            var output = string.Format(format, arg);
            _steps.Add(new WriteCommand(output));
        }

        public void WrapLine(string format, params object[] arg)
        {
            Wrap(format, arg);
            WriteLine();
        }

        public void Wrap(string format, params object[] arg)
        {
            var output = string.Format(format, arg);
            _steps.Add(new WrapCommand(output));
        }

        public void Write(IConsoleRenderer renderableData)
        {
            _steps.Add(new RenderCommand(renderableData));
        }

        public void WriteLine(IConsoleRenderer renderableData)
        {
            Write(renderableData);
            WriteLine();
        }

        public void FormatTable<T>(IEnumerable<T> items, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnSeperator = null)
        {
            _steps.Add(new FormatTableCommand<T, T>(items, options, columnSeperator));
        }

        public void FormatTable<T>(Report<T> report)
        {
            _steps.Add(FormatTableCommandFactory.Make(report));
        }

        public void WriteLine()
        {
            _steps.Add(new NewLineCommand());
        }

        public string ReadLine()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> Render(int width, out int wrappedLines)
        {
            var buffer = RenderToBuffer(width);
            wrappedLines = buffer.WordWrapLineBreakCount;
            return buffer.ToLines();
        }

        public int GetFirstWordLength(int tabLength)
        {
            if (_steps.Any())
                return _steps.First().GetFirstWordLength(tabLength);
            return 0;
        }

        public int GetLongestWordLength(int tabLength)
        {
            return _steps.Max(s => s.GetLongestWordLength(tabLength));
        }

        public int CountWordWrapLineBreaks(ColumnFormat format, int width)
        {
            return RenderToBuffer(width).WordWrapLineBreakCount;
        }

        private ReplayBuffer RenderToBuffer(int width)
        {
            var buffer = new ReplayBuffer(width);
            foreach (var recordedCommand in _steps)
            {
                recordedCommand.Replay(buffer);
            }

            return buffer;
        }
    }


}
