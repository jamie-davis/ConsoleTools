// ReSharper disable InconsistentNaming
namespace VT100.Utilities.ReadConsole
{
    public enum ResolvedCode
    {
        NotRecognised,
        CursorUp,
        CursorDown,
        CursorBackwards,
        CursorForward,
        PF1,
        PF2,
        PF3,
        PF4,
        PF5,
        PF6,
        PF7,
        PF8,
        PF9,
        PF10,
        PF11,
        PF12,
        PF13,
        PF14,
        PF15,
        PF16,
        PF17,
        PF18,
        PF19,
        PF20,
        Home,
        End,
        Delete,
        Insert,
        PageDown,
        PageUp,
        Begin,
        CR,
        Tab,
        Space,
        Backspace,
        CPR, //Cursor position report
        Escape
    }
}