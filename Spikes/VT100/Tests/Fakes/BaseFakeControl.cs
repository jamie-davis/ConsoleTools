using System;
using System.Collections.Generic;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Utilities.ReadConsole;

namespace VT100.Tests.Fakes
{
    internal class BaseFakeControl : ILayoutControl
    {
        private string _caption;
        private BorderBorderStyle _borderBorderStyle;
        private BoxRegion[] _boxRegions = {};

        #region Implementation of ILayoutControl

        public int Column { get; }

        public int Row { get; }

        public int Width { get; }

        public int Height { get; }

        public IEnumerable<BoxRegion> BoxRegions => _boxRegions;

        public void PropertyBind(IFullScreenApplication app, object layout, Func<object, object> getter,
            Action<object, object> setter)
        {
            throw new NotImplementedException();
        }

        public void MethodBind(IFullScreenApplication app, object layout, Func<object, bool> method)
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