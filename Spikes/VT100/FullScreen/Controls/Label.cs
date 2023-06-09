using System;
using VT100.FullScreen.ControlBehaviour;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen.Controls
{
    internal class Label : IFormattedLayoutControl<LabelFormat>
    {
        private BorderBorderStyle _borderBorderStyle;
        private LabelFormat _format;
        private IFullScreenApplication _app;
        private ILayout _dataContainer;
        private Func<object, object> _getter;
        private string _value;

        #region Implementation of ILayoutControl

        public int Column { get; private set; }

        public int Row { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public void PropertyBind(IFullScreenApplication app, ILayout layout, Func<object, object> getter, Action<object, object> _)
        {
            _getter = getter;
            _dataContainer = layout;
            _app = app;
            LoadValue();
        }

        private void LoadValue()
        {
            if (_dataContainer != null && _getter != null)
                _value = ControlValueLoader.GetString(_getter, _dataContainer);
            else
                _value = Caption;
        }

        public void MethodBind(IFullScreenApplication app, ILayout layout, Func<object, bool> method)
        {
            //Not possible for a label
        }

        public string Caption { get; set; }

        public void Render(IFullScreenConsole console)
        {
            console.SetCursorPosition(Column, Row);
            var visibleValue = _value;
            if (visibleValue.Length > Width)
                visibleValue = visibleValue.Substring(0, Width);
            else if (visibleValue.Length < Width)
                visibleValue = visibleValue.PadRight(Width);
            var format = new DisplayFormat()
            {
                Background = Format?.LabelBackground ?? VtColour.NoColourChange,
                Foreground = Format?.LabelForeground ?? VtColour.NoColourChange,
            };

            console.Write(visibleValue, format);
        }

        public void Position(int column, int row, int width, int height)
        {
            Column = column;
            Row = row;
            Width = width;
            Height = height;
        }

        public void SetFocus(IFullScreenConsole console)
        {
            //not possible for a label
        }

        public void Accept(IFullScreenConsole console, ControlSequence next)
        {
            //not possible for a label
        }

        public BorderBorderStyle BorderBorderStyle => _borderBorderStyle;

        public void Refresh(IFullScreenConsole console)
        {
            LoadValue();
            Render(console);
        }

        #endregion

        #region Implementation of IFormattedLayoutControl<LabelFormat>

        public LabelFormat Format
        {
            get => _format;
            set => _format = value;
        }

        #endregion
    }
    
    internal class LabelFormat
    {
        public VtColour LabelBackground { get; set; }
        public VtColour LabelForeground { get; set; }
    }
}