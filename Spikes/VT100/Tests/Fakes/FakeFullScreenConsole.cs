using System;
using System.Text;
using VT100.FullScreen;

namespace VT100.Tests.Fakes
{
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

        public void Write(string text)
        {
            foreach (var character in text)
            {
                Write(character);
            }
        }

        public void Write(char? character)
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

        public string GetDisplayReport()
        {
            var sb = new StringBuilder();
            var lineArray = new char[_windowWidth]; 
            for (var row = 0; row < _windowHeight; row++)
            {
                var sourceOffset = row * _windowWidth;
                Buffer.BlockCopy(_screenBuffer, sourceOffset * sizeof(char), lineArray, 0, _windowWidth * sizeof(char));
                if (row == _cursorY)
                    sb.Append('>');
                else
                    sb.Append('|');
                sb.AppendLine(new string(lineArray));
            }

            sb.AppendLine(" " + new string('-', _windowWidth));
            sb.AppendLine(" " + new string(' ', _cursorX) + "^");
            sb.AppendLine($"Cursor position: Row {_cursorY}, Column {_cursorX}");
            
            return sb.ToString();
        }
    }
}