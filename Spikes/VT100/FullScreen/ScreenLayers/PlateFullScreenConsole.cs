namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// Implementation of <see cref="IFullScreenConsole"/> that renders output to a <see cref="Plate"/>. This
    /// is used to assemble the layered display 
    /// </summary>
    internal class PlateFullScreenConsole : IFullScreenConsole
    {
        private int _windowWidth;
        private int _windowHeight;
        private int _cursorX;
        private int _cursorY;
        private int _writeIndex;

        public Plate Plate { get; }
        
        public PlateFullScreenConsole(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _cursorX = 0;
            _cursorY = 0;
            _writeIndex = 0;
            Plate = new Plate(windowWidth, windowHeight);
        }
        
        #region Implementation of IFullScreenConsole

        public void Write(string text)
        {
            var available = Plate.Width - _cursorX;
            var fittedText = text.Length > available ? text.Substring(0, available) : text;
            if (fittedText.Length > 0)
            {
                Plate.WriteText(_cursorX, _cursorY, fittedText);
                _cursorX += fittedText.Length;
            }
        }

        public void Write(char? character)
        {
            if (_cursorX < Plate.Width)
            {
                Plate.WriteText(_cursorX, _cursorY, character?.ToString() ?? " ");
                _cursorX++;
            }
        }

        public void SetCursorPosition(int column, int row)
        {
            _cursorX = column;
            _cursorY = row;
            _writeIndex = CharacterArrayIndexCalculator.GetIndex(_cursorX, _cursorY, Plate.Width);
        }

        public int WindowWidth => _windowWidth;

        public int WindowHeight => _windowHeight;

        #endregion
    }
}