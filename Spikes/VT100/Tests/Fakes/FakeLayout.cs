using VT100.FullScreen;

namespace VT100.Tests.Fakes
{
    internal class FakeLayout : ILayout
    {
        #region Implementation of ILayout

        public event LayoutUpdated LayoutUpdated;

        #endregion
    }
}