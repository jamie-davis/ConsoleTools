namespace VT100.Utilities
{
    internal interface IVTModeControl
    {
        void ToggleInsertMode();
        bool InsertModeOn { get; }
    }
}