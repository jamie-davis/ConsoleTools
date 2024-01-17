using System;
using System.Collections.Generic;
using System.Linq;
using VT100.Attributes;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen.Controls
{
    [Control(typeof(TextBoxAttribute))]
    internal class TextBoxControl : IFormattedLayoutControl<TextBoxFormat>
    {
        private object _dataContainer;
        private Func<object, object> _getter;
        private Action<object, object> _setter;

        private string _value;
        private TextBoxAttribute _attribute;
        private IFullScreenApplication _app;
        private CursorController _cursorControl;
        private readonly BorderBorderStyle _borderBorderStyle = new();
        private BoxRegion[] _boxRegions = {};

        // ReSharper disable once UnusedMember.Global
        internal void AcceptConfig(TextBoxAttribute attribute)
        {
            _attribute = attribute;
        }
        
        #region Implementation of ILayoutControl

        public int Column { get; private set; }

        public int Row { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public IEnumerable<BoxRegion> BoxRegions => _boxRegions;

        public void PropertyBind(IFullScreenApplication app, object layout, Func<object, object> getter,
            Action<object, object> setter)
        {
            _app = app;
            _dataContainer = layout;
            _getter = getter;
            _setter = setter;
            LoadValue();
        }

        public void MethodBind(IFullScreenApplication app, object layout, Func<object, bool> method)
        {
            //Not possible for a textbox
        }

        #endregion

        public string Caption => _attribute?.Caption;
        public int CharacterOffset { get; private set; }

        public void Render(IFullScreenConsole console)
        {
            console.SetCursorPosition(Column, Row);
            var visibleValue = _value;
            if (CharacterOffset > 0)
            {
                if (CharacterOffset < _value.Length)
                    visibleValue = _value.Substring(CharacterOffset);
            }
            
            if (visibleValue.Length > Width)
                visibleValue = visibleValue.Substring(0, Width);
            else if (visibleValue.Length < Width)
                visibleValue = visibleValue.PadRight(Width);

            var format = new DisplayFormat()
            {
                Background = Format?.InputBackground ?? VtColour.NoColourChange,
                Foreground = Format?.InputForeground ?? VtColour.NoColourChange,
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
            console.SetCursorPosition(Column, Row);
            var maxCharacterOffset = _value.Length - Width + 1;
            _cursorControl = new CursorController(0, 0, Width - 1, Column, Row, maxCharacterOffset > 0 ? maxCharacterOffset : 0, _value.Length);
            _cursorControl.MoveCursor += OnMoveCursor;
            _app.GotFocus(this);
        }

        private void OnMoveCursor(object? sender, (IFullScreenConsole Console, int X, int Y, int CharacterOffset) e)
        {
            if (!ReferenceEquals(sender, _cursorControl)) return;

            CharacterOffset = e.CharacterOffset;
            
            using (new CursorHider())
            {
                Render(e.Console);
                e.Console.SetCursorPosition(e.X, e.Y);
            }
        }

        public void Accept(IFullScreenConsole console, ControlSequence next)
        {
            if (_cursorControl.CursorControl(console, next)) return;

            if (next.ResolvedCode == ResolvedCode.NotRecognised)
            {
                TryAddCharacter(console, next);
            }
            else if (next.ResolvedCode == ResolvedCode.Backspace)
            {
                TryBackspace(console);
            }
            else if (next.ResolvedCode == ResolvedCode.Delete)
            {
                TryDelete(console);
            }
            _cursorControl.SetDataLength(_value.Length);            
        }

        public BorderBorderStyle BorderBorderStyle => _borderBorderStyle;
        
        public void Refresh(IFullScreenConsole console)
        {
            LoadValue();
            Render(console);
        }

        private void TryAddCharacter(IFullScreenConsole console, ControlSequence next)
        {
            var nextChar = next.Items.FirstOrDefault()?.KeyChar;
            if (nextChar == null) return;

            console.Write(nextChar);
            var characterPosition = _cursorControl.GetCharacterPosition();
            if (characterPosition > _value.Length)
            {
                _value = _value.PadRight(characterPosition) + nextChar;
            }
            else
            {
                if (_app.IsCursorModeInsert())
                    _value = _value.Substring(0, characterPosition) + nextChar + (characterPosition <= _value.Length
                        ? _value.Substring(characterPosition)
                        : string.Empty);
                else
                    _value = _value.Substring(0, characterPosition) + nextChar + (characterPosition < _value.Length
                        ? _value.Substring(characterPosition + 1)
                        : string.Empty);
            }

            SaveValue();
            _cursorControl.AdvanceCursor(console);
        }

        private void TryBackspace(IFullScreenConsole console)
        {
            var characterPosition = _cursorControl.GetCharacterPosition();
            if (characterPosition == 0) return;

            _value = _value.Substring(0, characterPosition - 1) + _value.Substring(characterPosition);
            SaveValue();
            _cursorControl.MoveCursorBack(console);
        }

        private void TryDelete(IFullScreenConsole console)
        {
            var characterPosition = _cursorControl.GetCharacterPosition();
            if (characterPosition >= _value.Length) return;
            
            _value = _value.Substring(0, characterPosition) + _value.Substring(characterPosition + 1);
            SaveValue();
            _cursorControl.RefreshCursor(console);
        }

        private void LoadValue()
        {
            _value = ControlValueLoader.GetString(_getter, _dataContainer);
        }

        private void SaveValue()
        {
            if (_dataContainer != null && _setter != null)
            {
                _setter(_dataContainer, _value);
            }
        }

        #region Implementation of IFormattedLayoutControl<TextBoxFormat>

        public TextBoxFormat Format { get; set; }
        public (int Width, int Height) GetMinSize()
        {
            return (1, 1);
        }

        public (int Width, int Height) GetMaxSize(int visibleWidth, int visibleHeight)
        {
            return (visibleWidth, 1);
        }

        #endregion
    }

    internal class TextBoxFormat
    {
        public VtColour InputBackground { get; set; }
        public VtColour InputForeground { get; set; }
    }
}