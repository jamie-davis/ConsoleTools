using System;
using System.Text;
using Vt100.FullScreen;

namespace VT100.Tests.Fakes
{
    internal class FakeFullScreenConsole : IFullScreenConsole
    {
        private readonly int _columns;
        private readonly int _rows;
        private readonly char[,] _screenBuffer;
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private int _cursorX;
        private int _cursorY;

        public FakeFullScreenConsole(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;
            _cursorX = 0;
            _cursorY = 0;
            _screenBuffer = new char[rows, columns];
            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
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
            if (_cursorX >= _columns)
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
            if (_cursorY == _rows)
            {
                for (var copyToRow = 0; copyToRow < _rows - 1; copyToRow++)
                {
                    var sourceOffset = (copyToRow + 1) * _columns;
                    var destOffset = copyToRow * _columns;
                    Buffer.BlockCopy(_screenBuffer, sourceOffset, _screenBuffer, destOffset, _columns * sizeof(char));
                }

                for (var col = 0; col < _columns; ++col)
                    _screenBuffer[_rows - 1, col] = ' ';
            }
            else
            {
                ++_cursorY;
            }
        }

        public string GetDisplayReport()
        {
            var sb = new StringBuilder();
            var lineArray = new char[_columns]; 
            for (var row = 0; row < _rows; row++)
            {
                var sourceOffset = row * _columns;
                Buffer.BlockCopy(_screenBuffer, sourceOffset, lineArray, 0, _columns * sizeof(char));
                if (row == _cursorY)
                    sb.Append('>');
                else
                    sb.Append('|');
                sb.AppendLine(new string(lineArray));
            }

            sb.AppendLine(" " + new string('-', _columns));
            sb.AppendLine(" " + new string(' ', _cursorX) + "^");
            sb.AppendLine($"Cursor position: Row {_cursorY}, Column {_cursorX}");
            
            return sb.ToString();
        }
    }
}