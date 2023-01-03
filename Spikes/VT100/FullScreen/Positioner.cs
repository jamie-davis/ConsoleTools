using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.Controls;

namespace VT100.FullScreen
{
    internal class Positioner
    {
        private readonly IFullScreenConsole _console;

        private class ControlContainer
        {
            public ReadOnlyCollection<PropertySetting> PropertySettings { get; }
            private readonly ILayoutControl _control;

            public string Caption => _control.Caption ?? string.Empty;
            public ControlContainer(ILayoutControl control, ReadOnlyCollection<PropertySetting> propertySettings)
            {
                PropertySettings = propertySettings;
                _control = control;
            }

            public int CaptionX { get; set; }
            public int CaptionY { get; set; }
            public string CaptionText { get; set; }

            public ILayoutControl Control => _control;
        }

        public int Row { get; }
        public int Column { get; }

        public int Width { get; }
        public int Height { get; }
        public CaptionAlignment CaptionAlignment { get; }

        private List<ControlContainer> _combinedControls;
        private List<ControlContainer> _controls;
        private List<ControlContainer> _buttons;

        public Positioner(int width, int height, CaptionAlignment captionAlignment, IEnumerable<LayedOutControl> controls, IFullScreenConsole console)
        {
            _console = console;
            Width = width;
            Height = height;
            CaptionAlignment = captionAlignment;

            Row = 1;
            Column = 1;

            var allControls = controls.Select(c => new ControlContainer(c.Control, c.PropertySettings)).ToList();
            _controls = allControls.Where(c => !(c.Control is ButtonControl)).ToList();
            _buttons = allControls.Where(c => c.Control is ButtonControl).ToList();
            _combinedControls = _controls.Concat(_buttons).ToList();

            var maxAllowableCaption = width - 4;
            var maxCaption = Math.Min(_controls.Max(c => c.Caption.Length), maxAllowableCaption);
            foreach (var container in _controls)
            {
                container.CaptionText = $"{container.Caption.PadRight(maxCaption)}:";
                container.CaptionX = Column;
                container.CaptionY = Row;
                var controlX = Column + maxCaption + 2;
                container.Control.Position(controlX, Row, Width - controlX - 1, 1);

                Row += 2;
            }

            var space = _buttons.Select(b => b.Control.GetRequestedSize()).ToList();
            var maxHeight = space.Max(m => (int?)m.Height) ?? 0;
            var fullWidth = (space.Sum(m => (int?)m.Width) ?? 0) + space.Count - 1;
            Row += maxHeight;
            var column = (Width - fullWidth)/2;
            foreach (var container in _buttons) 
            {
                container.CaptionText = $"{container.Caption.PadRight(maxCaption)}:";
                var requestedSize = container.Control.GetRequestedSize();
                var controlY = Row - requestedSize.Height;
                var controlX = column;
                column += requestedSize.Width + 1;
                container.Control.Position(controlX, controlY, requestedSize.Width, requestedSize.Height);
            }
        }

        public void Render(List<PropertySetting> settings)
        {
            using (new CursorHider())
            {
                foreach (var container in _controls)
                {
                    _console.SetCursorPosition(container.CaptionX, container.CaptionY);
                    _console.Write(container.CaptionText);
                    ControlSettingsUpdater.Update(container.Control, settings, container.PropertySettings);
                    container.Control.Render(_console);
                }
                
                foreach (var container in _buttons)
                {
                    container.Control.Render(_console);
                }
            }
        }

        public void SetFocus(IFullScreenConsole console)
        {
            _controls.FirstOrDefault()?.Control?.SetFocus(console);
        }

        public void NextFocus(IFullScreenConsole console, ILayoutControl layoutControl)
        {
            var focusContainer = _combinedControls.FirstOrDefault(c => ReferenceEquals(c.Control, layoutControl));
            if (focusContainer == null)
            {
                SetFocus(console);
                return;
            }
            var index = _combinedControls.IndexOf(focusContainer);
            if (index + 1 >= _combinedControls.Count)
            {
                SetFocus(console);
                return;
            }

            var control = _combinedControls[index + 1];
            control.Control?.SetFocus(console);
        }

        public void ReRender(IFullScreenConsole console)
        {
            foreach (var control in _combinedControls)
            {
                control.Control.Refresh(console);
            }
        }
    }
}