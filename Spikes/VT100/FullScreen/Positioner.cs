using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.Controls;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    internal class Positioner
    {
        private readonly IFullScreenConsole _console;

        class LayoutProperties
        {
            public IBorderStyle Border { get; set; }

            public bool HasBorder()
            {
                return Border != null
                       && (Border.TopBorder != BorderType.None
                           || Border.BottomBorder != BorderType.None
                           || Border.LeftBorder != BorderType.None
                           || Border.RightBorder != BorderType.None);
            }
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

            public int Column => LayoutProperties.HasBorder() ? Label.Column - 1 : Label.Column;
            public int Row => LayoutProperties.HasBorder() ? Label.Row - 1 : Label.Row;
            public int Width
            {
                get
                {
                    var controlWidth = Control.Column - Label.Column + Control.Width;
                    return LayoutProperties.HasBorder() ? controlWidth + 2 : controlWidth;
                }
            }
            public int Height => LayoutProperties.HasBorder() ? Control.Height + 2 : Control.Height;

            public void RenderBorder(IFullScreenConsole console)
            {
                var regions = new[] { new BoxRegion(Column, Row, Width, Height, LineWeight.Light) };
                var map = BoxMapMaker.Map(regions, console.WindowWidth, console.WindowHeight);
                DisplayFormat format = new ()
                {
                    Foreground = VtColour.NoColourChange,
                    Background = VtColour.NoColourChange,
                };
                BoxRenderer.RenderMapToConsole(map, console, format);
            }
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
                var captionedContainers = _controls.Where(c => !c.LayoutProperties.HasBorder());
                return Math.Min(captionedContainers.Max(c => c.CaptionText.Length), maxAllowableCaption);
            }

            var maxCaption = GetLongestCaptionLength();
            foreach (var container in _controls)
            {
                container.Label.Caption = container.CaptionText;
                var inBorder = container.LayoutProperties.HasBorder();

                var captionLength = inBorder ? container.Label.Caption.Length : maxCaption;
                var captionCol = inBorder ? Column + 1 : Column;
                var captionRow = inBorder ? Row + 1 : Row;
                container.Label.Position(captionCol, captionRow, captionLength, 1);

                var controlX = captionCol + captionLength + 2;
                var controlWidth = Width - controlX - (inBorder ? 2 : 1);
                container.Control.Position(controlX, captionRow, controlWidth, 1);

                Row += inBorder ? 4 : 2;
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

            foreach (var borderedContainer in _controls.Where(c => c.LayoutProperties.HasBorder()))
            {
                borderedContainer.RenderBorder(console);
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