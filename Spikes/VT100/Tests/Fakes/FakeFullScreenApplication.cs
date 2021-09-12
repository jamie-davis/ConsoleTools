using Vt100.FullScreen;

namespace VT100.Tests.Fakes
{
    internal class FakeFullScreenApplication : IFullScreenApplication
    {
        #region Implementation of IFullScreenApplication

        public void GotFocus(ILayoutControl focusControl)
        {
            
        }

        public bool IsCursorModeInsert()
        {
            return InsertModeOn;
        }

        public bool InsertModeOn { get; set; } = true;

        #endregion
    }
}