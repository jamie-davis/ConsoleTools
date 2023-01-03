using System;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.Utilities.ReadConsole;

namespace VT100.Tests.Fakes
{
    internal class BaseFakeControl : ILayoutControl
    {
        private string _caption;
        private BorderBorderStyle _borderBorderStyle;

        #region Implementation of ILayoutControl

        public void PropertyBind(IFullScreenApplication app, ILayout layout, Func<object, object> getter, Action<object, object> setter)
        {
            throw new NotImplementedException();
        }

        public void MethodBind(IFullScreenApplication app, ILayout layout, Func<object, bool> method)
        {
            throw new NotImplementedException();
        }

        public string Caption => _caption;

        public void Render(IFullScreenConsole console)
        {
            throw new NotImplementedException();
        }

        public void Position(int column, int row, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetFocus(IFullScreenConsole console)
        {
            throw new NotImplementedException();
        }

        public void Accept(IFullScreenConsole console, ControlSequence next)
        {
            throw new NotImplementedException();
        }

        public BorderBorderStyle BorderBorderStyle => _borderBorderStyle;

        public void Refresh(IFullScreenConsole console)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}