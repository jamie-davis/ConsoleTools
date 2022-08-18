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
        private readonly int _cursorX;
        private readonly int _cursorY;

        public Plate Plate { get; }
        
        public PlateFullScreenConsole(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _cursorX = 0;
            _cursorY = 0;
            Plate = new Plate(windowWidth, windowHeight);
        }
        
        #region Implementation of IFullScreenConsole

        public void Write(string text)
        {
            throw new System.NotImplementedException();
        }

        public void Write(char? character)
        {
            throw new System.NotImplementedException();
        }

        public void SetCursorPosition(int column, int row)
        {
            throw new System.NotImplementedException();
        }

        public int WindowWidth => _windowWidth;

        public int WindowHeight => _windowHeight;

        #endregion
    }
}