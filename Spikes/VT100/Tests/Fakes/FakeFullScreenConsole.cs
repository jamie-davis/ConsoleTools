using System;
using System.Diagnostics;
using System.Text;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;

namespace VT100.Tests.Fakes
{
    [DebuggerDisplay("{GetDisplayReport(DisplayReportOptions.NoDiagnostics)}")]
    internal class FakeFullScreenConsole : IFullScreenConsole
    {
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly char[,] _screenBuffer;
        private int _cursorX;
        private int _cursorY;

        public FakeFullScreenConsole(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _cursorX = 0;
            _cursorY = 0;
            _screenBuffer = new char[windowHeight, windowWidth];
            for (var row = 0; row < windowHeight; row++)
            {
                for (var column = 0; column < windowWidth; column++)
                {
                    _screenBuffer[row, column] = ' ';
                }
            }
        }

        #region Implementation of IFullScreenConsole

        public void Write(string text, DisplayFormat format = default)
        {
            foreach (var character in RawTextExtractor.Extract(text))
            {
                Write(character);
            }
        }
        
        public void Write(char? character, DisplayFormat format = default)
        {
            if (_cursorX >= _windowWidth)
            {
                CursorToNextLine();
            }

            _screenBuffer[_cursorY, _cursorX++] = character ?? ' ';
        }

        public void SetCursorPosition(int column, int row)
        {
            _cursorX = column;
            _cursorY = row;
        }

        public int WindowWidth => _windowWidth;

        public int WindowHeight => _windowHeight;
        public void SetCharacter(int column, int row, char character, DisplayFormat format)
        {
            if (column >= _windowWidth || column < 0 || row >= _windowHeight || row < 0) return;

            var oldCol = _cursorX;
            var oldRow = _cursorY;
            _cursorX = column;
            _cursorY = row;
            Write(character, format);
            _cursorX = oldCol;
            _cursorY = oldRow;
        }

        #endregion

        private void CursorToNextLine()
        {
            _cursorX = 0;
            if (_cursorY == _windowHeight)
            {
                for (var copyToRow = 0; copyToRow < _windowHeight - 1; copyToRow++)
                {
                    var sourceOffset = (copyToRow + 1) * _windowWidth;
                    var destOffset = copyToRow * _windowWidth;
                    Buffer.BlockCopy(_screenBuffer, sourceOffset, _screenBuffer, destOffset, _windowWidth * sizeof(char));
                }

                for (var col = 0; col < _windowWidth; ++col)
                    _screenBuffer[_windowHeight - 1, col] = ' ';
            }
            else
            {
                ++_cursorY;
            }
        }

        public string GetDisplayReport(DisplayReportOptions options = DisplayReportOptions.Default)
        {
            var sb = new StringBuilder();
            var lineArray = new char[_windowWidth]; 
            for (var row = 0; row < _windowHeight; row++)
            {
                var sourceOffset = row * _windowWidth;
                Buffer.BlockCopy(_screenBuffer, sourceOffset * sizeof(char), lineArray, 0, _windowWidth * sizeof(char));
                if (row == _cursorY && options == DisplayReportOptions.Default)
                    sb.Append('>');
                else if (options == DisplayReportOptions.Default)
                    sb.Append('|');
                sb.AppendLine(new string(lineArray));
            }

            if (options == DisplayReportOptions.Default)
            {
                sb.AppendLine(" " + new string('-', _windowWidth));
                sb.AppendLine(" " + new string(' ', _cursorX) + "^");
                sb.AppendLine($"Cursor position: Row {_cursorY}, Column {_cursorX}");
            }

            return sb.ToString();
        }
    }

    internal static class DebugConsole
    {
        public static string Render(IFullScreenConsole console)
        {
            if (!(console is FakeFullScreenConsole fake)) return "Not a fake.";

            return fake.GetDisplayReport();
        }

    }
    
    internal enum DisplayReportOptions
    {
        Default,
        NoDiagnostics
    }
}