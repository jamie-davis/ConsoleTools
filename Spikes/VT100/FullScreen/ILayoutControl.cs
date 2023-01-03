using System;
using VT100.FullScreen.ControlBehaviour;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen
{
    internal interface ILayoutControl
    {
        void PropertyBind(IFullScreenApplication app, ILayout layout, Func<object, object> getter, Action<object, object> setter);
        void MethodBind(IFullScreenApplication app, ILayout layout, Func<object, bool> method);
        string Caption { get; }
        void Render(IFullScreenConsole console);

        (int Width, int Height) GetRequestedSize()
        {
            return (1, 1);
        }
        
        void Position(int column, int row, int width, int height);
        void SetFocus(IFullScreenConsole console);
        void Accept(IFullScreenConsole console, ControlSequence next);
        
        BorderBorderStyle BorderBorderStyle { get; }
        void Refresh(IFullScreenConsole console);
    }

    interface IFormattedLayoutControl<TFormat> : ILayoutControl 
        where TFormat : class, new()
    {
        public TFormat Format { get; set; }
    }
}