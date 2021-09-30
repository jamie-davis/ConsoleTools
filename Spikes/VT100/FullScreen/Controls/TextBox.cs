using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Vt100.Attributes;
using VT100.Utilities.ReadConsole;

namespace Vt100.FullScreen.Controls
{
    [Control(typeof(TextBoxAttribute))]
    internal class TextBox : ILayoutControl
    {
        private ILayout _dataContainer;
        private Func<object, object> _getter;
        private Action<object, object> _setter;

        private string _value;
        private TextBoxAttribute _attribute;
        private int _column;
        private int _row;
        private int _width;
        private int _height;
        private IFullScreenApplication _app;
        private CursorController _cursorControl;

        internal void AcceptConfig(TextBoxAttribute attribute)
        {
            _attribute = attribute;
        }
        
        #region Implementation of ILayoutControl

        public void Bind(IFullScreenApplication app, ILayout layout, Func<object, object> getter, Action<object, object> setter)
        {
            _app = app;
            _dataContainer = layout;
            _getter = getter;
            _setter = setter;
            LoadValue();
        }

        #endregion

        public string Caption => _attribute?.Caption;
        public int CharacterOffset { get; private set; }

        public void Render()
        {
            _app.Console.SetCursorPosition(_column, _row);
            var visibleValue = _value;
            if (CharacterOffset > 0)
            {
                if (CharacterOffset < _value.Length)
                    visibleValue = _value.Substring(CharacterOffset);
            }
            
            if (visibleValue.Length > _width)
                visibleValue = visibleValue.Substring(0, _width);
            else if (visibleValue.Length < _width)
                visibleValue = visibleValue.PadRight(_width);

            _app.Console.Write(visibleValue);
        }
        
        public void Position(int column, int row, int width, int height)
        {
            _column = column;
            _row = row;
            _width = width;
            _height = height;
        }

        public void SetFocus()
        {
            _app.Console.SetCursorPosition(_column, _row);
            var maxCharacterOffset = _value.Length - _width + 1;
            _cursorControl = new CursorController(0, 0, _width - 1, _column, _row, maxCharacterOffset > 0 ? maxCharacterOffset : 0, _value.Length);
            _cursorControl.MoveCursor += OnMoveCursor;
            _app.GotFocus(this);
        }

        private void OnMoveCursor(object? sender, (int X, int Y, int CharacterOffset) e)
        {
            if (!ReferenceEquals(sender, _cursorControl)) return;

            CharacterOffset = e.CharacterOffset;
            
            using (new CursorHider())
            {
                Render();
                _app.Console.SetCursorPosition(e.X, e.Y);
            }
        }

        public void Accept(ControlSequence next)
        {
            if (_cursorControl.CursorControl(next)) return;

            if (next.ResolvedCode == ResolvedCode.NotRecognised)
            {
                TryAddCharacter(next);
            }
            else if (next.ResolvedCode == ResolvedCode.Backspace)
            {
                TryBackspace();
            }
            else if (next.ResolvedCode == ResolvedCode.Delete)
            {
                TryDelete();
            }
            _cursorControl.SetDataLength(_value.Length);            
        }

        private void TryAddCharacter(ControlSequence next)
        {
            var nextChar = next.Items.FirstOrDefault()?.KeyChar;
            if (nextChar == null) return;

            _app.Console.Write(nextChar);
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
            _cursorControl.AdvanceCursor();
        }

        private void TryBackspace()
        {
            var characterPosition = _cursorControl.GetCharacterPosition();
            if (characterPosition == 0) return;

            _value = _value.Substring(0, characterPosition - 1) + _value.Substring(characterPosition);
            SaveValue();
            _cursorControl.MoveCursorBack();
        }

        private void TryDelete()
        {
            var characterPosition = _cursorControl.GetCharacterPosition();
            if (characterPosition >= _value.Length) return;
            
            _value = _value.Substring(0, characterPosition) + _value.Substring(characterPosition + 1);
            SaveValue();
            _cursorControl.RefreshCursor();
        }

        private void LoadValue()
        {
            if (_dataContainer != null && _getter != null)
                _value = GetString(_getter(_dataContainer));
            else
                _value = null;
        }

        private string GetString(object objValue)
        {
            if (objValue is string strValue)
                return strValue;

            return objValue?.ToString() ?? string.Empty;
        }

        private void SaveValue()
        {
            if (_dataContainer != null && _setter != null)
            {
                _setter(_dataContainer, _value);
            }
        }
    }
}