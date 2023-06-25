using System;
using System.Collections;
using System.Collections.Generic;
using VT100.Attributes;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen.Controls
{
    [Control(typeof(ButtonAttribute), bindsToProperty:false)]
    internal class ButtonControl : IFormattedLayoutControl<ButtonFormat>
    {
        private ButtonAttribute _attribute;
        private Func<object, bool> _method;
        private object _dataContainer;
        private IFullScreenApplication _app;
        private int _captionRow;
        private int _captionColumn;
        private BoxRegion _boxRegion;

        public int Column { get; private set; }

        public int Row { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public IEnumerable<BoxRegion> BoxRegions => _boxRegion == null ? new BoxRegion[] {} : new [] { _boxRegion };

        public void PropertyBind(IFullScreenApplication app, object layout, Func<object, object> getter,
            Action<object, object> setter)
        {
            //Not possible for a button
        }

        public void MethodBind(IFullScreenApplication app, object layout, Func<object, bool> method)
        {
            _app = app;
            _method = method;
            _dataContainer = layout;
        }

        public string Caption => _attribute.Caption;

        public void Render(IFullScreenConsole console)
        {
            var captionAvailableWidth = Width - 2;
            string caption;
            if (captionAvailableWidth > Caption.Length)
            {
                _captionColumn = Column + (captionAvailableWidth - Caption.Length) / 2;
                caption = Caption;
            }
            else
            {
                _captionColumn = Column + 1;
                caption = Caption.Substring(0, captionAvailableWidth);
            }

            if (Height > 2)
                _captionRow = Row + (Height - 1) / 2;
            else
                _captionRow = Row;

            var format = new DisplayFormat()
            {
                Background = Format?.ButtonBackground ?? VtColour.NoColourChange,
                Foreground = Format?.ButtonForeground ?? VtColour.NoColourChange,
            };
            
            console.SetCursorPosition(_captionColumn, _captionRow);
            console.Write(caption, format);
        }

        public (int Width, int Height) GetRequestedSize()
        {
            return (Caption.Length + 2, 3);
        }

        public void Position(int column, int row, int width, int height)
        {
            Column = column;
            Row = row;
            Width = width;
            Height = height;
            
            if (Width > Caption.Length + 1 && Height > 2)
            {
                _boxRegion = new BoxRegion(Column, Row, Width, Height, LineWeight.Heavy);
            }

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

        public BorderBorderStyle BorderBorderStyle { get; }
        public void Refresh(IFullScreenConsole console)
        {
            Render(console);
        }

        #region Implementation of IFormattedLayoutControl<ButtonFormat>

        public ButtonFormat Format { get; set; }

        #endregion
    }
}
internal class ButtonFormat
{
    [DefaultFrom(typeof(BackgroundAttribute))]
    public VtColour ButtonBackground { get; set; }
    
    [DefaultFrom(typeof(ForegroundAttribute))]
    public VtColour ButtonForeground { get; set; }
}
