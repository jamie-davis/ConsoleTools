using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.Controls;

namespace VT100.FullScreen
{
    internal class Positioner
    {
        private readonly IFullScreenConsole _console;

        class LayoutProperties
        {
            public IBorderStyle Border { get; set; }
        }

        private class ControlContainer
        {
            public ReadOnlyCollection<PropertySetting> PropertySettings { get; }
            private readonly ILayoutControl _control;

            public string CaptionText => _control.Caption ?? string.Empty;
            public ControlContainer(ILayoutControl control, ReadOnlyCollection<PropertySetting> propertySettings)
            {
                PropertySettings = propertySettings;
                _control = control;
                Label = new Label();
                LayoutProperties = new LayoutProperties();
                ControlPropertySetter.Set(LayoutProperties, propertySettings);
            }
            
            public Label Label { get; }
            public LayoutProperties LayoutProperties { get; }

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

            int GetLongestCaptionLength()
            {
                var captionedContainers = _controls.Where(c => c.LayoutProperties.Border == null);
                return Math.Min(captionedContainers.Max(c => c.CaptionText.Length), maxAllowableCaption);
            }

            var maxCaption = GetLongestCaptionLength();
            foreach (var container in _controls)
            {
                container.Label.Caption = container.CaptionText;
                if (container.LayoutProperties.Border != null)
                {
                    
                }
                
                container.Label.Position(Column, Row, maxCaption, 1);

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
                    ControlSettingsUpdater.Update(container.Control, settings, container.PropertySettings);
                    container.Label.Refresh(_console);
                    container.Control.Refresh(_console);
                }
                
                foreach (var container in _buttons)
                {
                    ControlSettingsUpdater.Update(container.Control, settings, container.PropertySettings);
                    container.Control.Refresh(_console);
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

        public void PrevFocus(IFullScreenConsole console, ILayoutControl layoutControl)
        {
            var focusContainer = _combinedControls.FirstOrDefault(c => ReferenceEquals(c.Control, layoutControl));
            if (focusContainer == null)
            {
                SetFocus(console);
                return;
            }
            var index = _combinedControls.IndexOf(focusContainer);
            var newIndex = index - 1;
            if (newIndex < 0 && _combinedControls.Count > 0) newIndex = _combinedControls.Count - 1;
            if (newIndex < 0) return;

            var control = _combinedControls[newIndex];
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