using Vt100.FullScreen;

namespace VT100.Tests.Fullscreen.Controls
{
    internal class FakeLayout : ILayout
    {
        #region Implementation of ILayout

        public event LayoutUpdated LayoutUpdated;

        #endregion
    }
}