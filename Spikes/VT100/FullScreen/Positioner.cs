using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace VT100.FullScreen
{
    internal class Positioner
    {
        private readonly IFullScreenConsole _console;

        private class ControlContainer
        {
            private readonly ILayoutControl _control;

            public string Caption => _control.Caption ?? string.Empty;
            public ControlContainer(ILayoutControl control)
            {
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

        private List<ControlContainer> _controls;

        public Positioner(int width, int height, CaptionAlignment captionAlignment, IEnumerable<ILayoutControl> controls, IFullScreenConsole console)
        {
            _console = console;
            Width = width;
            Height = height;
            CaptionAlignment = captionAlignment;

            Row = 1;
            Column = 1;

            _controls = controls.Select(c => new ControlContainer(c)).ToList();

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
        }

        public void Render()
        {
            using (new CursorHider())
            {
                foreach (var container in _controls)
                {
                    _console.SetCursorPosition(container.CaptionX, container.CaptionY);
                    _console.Write(container.CaptionText);
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
            var focusContainer = _controls.FirstOrDefault(c => ReferenceEquals(c.Control, layoutControl));
            if (focusContainer == null)
            {
                SetFocus(console);
                return;
            }
            var index = _controls.IndexOf(focusContainer);
            if (index + 1 >= _controls.Count)
            {
                SetFocus(console);
                return;
            }

            var control = _controls[index + 1];
            control.Control?.SetFocus(console);
        }
    }
}