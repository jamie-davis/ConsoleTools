using VT100.FullScreen;

namespace VT100.Tests.Fakes
{
    internal class FakeFullScreenApplication : IFullScreenApplication
    {
        public FakeFullScreenApplication(int columns = 80, int rows = 25)
        {
            Console = new FakeFullScreenConsole(columns, rows);
        }
        
        #region Implementation of IFullScreenApplication

        public void GotFocus(ILayoutControl focusControl)
        {
            
        }

        public bool IsCursorModeInsert()
        {
            return InsertModeOn;
        }

        public FakeFullScreenConsole Console { get; }

        public bool InsertModeOn { get; set; } = true;

        #endregion
    }
}