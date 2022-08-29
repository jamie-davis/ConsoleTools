using System;
using System.Collections;
using System.Collections.Generic;
using VT100.Attributes;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen.Controls
{
    [Control(typeof(ButtonAttribute), bindsToProperty:false)]
    internal class ButtonControl : ILayoutControl
    {
        private ButtonAttribute _attribute;
        private Func<object, bool> _method;
        private ILayout _dataContainer;
        private IFullScreenApplication _app;
        private int _column;
        private int _row;
        private int _width;
        private int _height;
        private int _captionRow;
        private int _captionColumn;

        public void PropertyBind(IFullScreenApplication app, ILayout layout, Func<object, object> getter, Action<object, object> setter)
        {
            //Not possible for a button
        }

        public void MethodBind(IFullScreenApplication app, ILayout layout, Func<object, bool> method)
        {
            _app = app;
            _method = method;
            _dataContainer = layout;
        }

        public string Caption => _attribute.Caption;

        public void Render(IFullScreenConsole console)
        {
            var captionAvailableWidth = _width - 2;
            string caption;
            if (captionAvailableWidth > Caption.Length)
            {
                _captionColumn = _column + (captionAvailableWidth - Caption.Length) / 2;
                caption = Caption;
            }
            else
            {
                _captionColumn = _column + 1;
                caption = Caption.Substring(0, captionAvailableWidth);
            }

            if (_height > 2)
                _captionRow = _row + (_height - 1) / 2;
            else
                _captionRow = _row;

            if (_width > Caption.Length + 1 && _height > 2)
            {
                var regions = new[] { new BoxRegion(_column, _row, _width, _height, LineWeight.Heavy) };
                var map = BoxMapMaker.Map(regions, console.WindowWidth, console.WindowHeight);
                BoxRenderer.RenderMapToConsole(map, console);
            }
            
            console.SetCursorPosition(_captionColumn, _captionRow);
            console.Write(caption);
        }

        public (int Width, int Height) GetRequestedSize()
        {
            return (Caption.Length + 2, 3);
        }

        public void Position(int column, int row, int width, int height)
        {
            _column = column;
            _row = row;
            _width = width;
            _height = height;
        }

        public void SetFocus(IFullScreenConsole console)
        {
            console.SetCursorPosition(_captionColumn, _captionRow);
            _app.GotFocus(this);
        }

        public void Accept(IFullScreenConsole console, ControlSequence next)
        {
            if (next.ResolvedCode == ResolvedCode.CR 
                || (next.ResolvedCode == ResolvedCode.NotRecognised 
                    && next.Items.Count == 1 && next.Items[0].KeyChar == ' '))
            {
                if (_method(_dataContainer) && _attribute?.ExitMode == ExitMode.ExitOnSuccess)
                    _app.CloseScreen();

                _app.ReRender();
            }
        }

        // ReSharper disable once UnusedMember.Global
        internal void AcceptConfig(ButtonAttribute attribute)
        {
            _attribute = attribute;
        }

        public Style Style { get; }
        public void Refresh(IFullScreenConsole console)
        {
            
        }
    }
}