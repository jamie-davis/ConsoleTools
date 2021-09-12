namespace Vt100.FullScreen
{
    internal interface IFullScreenApplication
    {
        void GotFocus(ILayoutControl focusControl);
        bool IsCursorModeInsert();
        IFullScreenConsole Console { get; }
    }
}