using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal.RecordedCommands
{
    internal class ReplayBuffer
    {
        private static readonly ColumnFormat DefaultColumnFormat = new ColumnFormat(null);
        public int Width { get; private set; }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        public int CursorLeft
        {
            get { return _currentLineLength; }
        }

        public int WordWrapLineBreakCount { get; private set; }

        private readonly List<string> _lines = new List<string>();
        private StringBuilder _currentLine;
        private int _currentLineLength;

        public ReplayBuffer(int width)
        {
            Width = width;
        }

        public IEnumerable<string> ToLines()
        {
            if (_currentLine == null)
                return _lines;

            return _lines.Concat(new[] {_currentLine.ToString()});
        }

        public void Write(string data)
        {
            var colourSplit = ColourControlSplitter.Split(data);
            foreach (var item in colourSplit)
            {
                Write(item);
            }
        }

        private void Write(ColourControlItem item)
        {
            if (_currentLine == null)
                StartLine();

            if (item.Text != null)
                WriteText(item.Text);
            else
            {
                var affinity = item.PrefixAffinity
                    ? AdapterConfiguration.PrefixAffinity
                    : (item.SuffixAffinity ? AdapterConfiguration.SuffixAffinity : string.Empty);
                var introducer = AdapterConfiguration.ControlSequenceIntroducer + affinity;
                var terminator = AdapterConfiguration.ControlSequenceTerminator;
                var instructions = string.Empty;
                foreach (var instruction in item.Instructions)
                {
                    if (instruction.Code == ColourControlItem.ControlCode.NewLine)
                    {
                        if (!string.IsNullOrEmpty(instructions))
                            Emit(introducer + instructions + terminator);

                        StartLine();
                    }
                    else
                        instructions += instruction.GenerateCode();
                }

                if (!string.IsNullOrEmpty(instructions))
                    Emit(introducer + instructions + terminator);
            }
        }

        private void WriteText(string data)
        {
            Debug.Assert(_currentLine != null);

            var dataLength = ColourString.Length(data);
            var dataPos = 0;
            while ((dataLength - dataPos) + _currentLineLength > Width)
            {
                var spaceOnLine = Width - _currentLineLength;
                Emit(ColourString.Substring(data, dataPos, spaceOnLine));
                dataPos += spaceOnLine;
                StartLine();
                ++WordWrapLineBreakCount;
            }

            _currentLine.Append(ColourString.Substring(data, dataPos));
            _currentLineLength += dataLength - dataPos;
        }

        private void Emit(string data)
        {
            if (_currentLine == null)
                StartLine();

            Debug.Assert(_currentLine != null);
            _currentLine.Append(data);
        }

        private void StartLine()
        {
            if (_currentLine != null)
                _lines.Add(_currentLine.ToString());

            _currentLineLength = 0;
            _currentLine = new StringBuilder();
        }

        public void NewLine()
        {
            StartLine();
        }

        public void Wrap(string data)
        {
            var lines = ColumnWrapper.WrapValue(data, DefaultColumnFormat, Width, firstLineHangingIndent: _currentLineLength);
            if (lines.Length == 0) return;

            foreach (var line in lines.Where((l, c) => c < lines.Length - 1))
            {
                Write(line);
                NewLine();
                ++WordWrapLineBreakCount;
            }
            Write(lines.Last());
        }

        /// <summary>
        /// Add additional wrapping line breaks.
        /// </summary>
        /// <param name="wrappedLineBreaks"></param>
        public void RecordWrapLineBreaks(int wrappedLineBreaks)
        {
            WordWrapLineBreakCount += wrappedLineBreaks;
        }
    }
}
