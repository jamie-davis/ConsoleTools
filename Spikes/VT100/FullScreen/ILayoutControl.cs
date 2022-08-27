using System;
using VT100.FullScreen.ControlBehaviour;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen
{
    internal interface ILayoutControl
    {
        void Bind(IFullScreenApplication app, ILayout layout, Func<object, object> getter, Action<object, object> setter);
        string Caption { get; }
        void Render(IFullScreenConsole console);
        void Position(int column, int row, int width, int height);
        void SetFocus(IFullScreenConsole console);
        void Accept(IFullScreenConsole console, ControlSequence next);
        
        Style Style { get; }
    }
}