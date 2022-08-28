namespace VT100.FullScreen
{
    internal interface IFullScreenApplication
    {
        void GotFocus(ILayoutControl focusControl);
        bool IsCursorModeInsert();
        void CloseScreen();
    }
}