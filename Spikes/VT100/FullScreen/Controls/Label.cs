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
        private int _column;
        private int _row;
        private int _width;
        private int _height;

        #region Implementation of ILayoutControl

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
            console.SetCursorPosition(_column, _row);
            var visibleValue = _value;
            if (visibleValue.Length > _width)
                visibleValue = visibleValue.Substring(0, _width);
            else if (visibleValue.Length < _width)
                visibleValue = visibleValue.PadRight(_width);
            var format = new DisplayFormat()
            {
                Background = Format?.LabelBackground ?? VtColour.NoColourChange,
                Foreground = Format?.LabelForeground ?? VtColour.NoColourChange,
            };

            console.Write(visibleValue, format);
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