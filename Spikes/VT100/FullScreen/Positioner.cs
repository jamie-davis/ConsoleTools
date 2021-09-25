using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace Vt100.FullScreen
{
    internal class Positioner
    {
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

        public Positioner(int width, int height, CaptionAlignment captionAlignment, IEnumerable<ILayoutControl> controls)
        {
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
            //hide
            using (new CursorHider())
            {
                foreach (var container in _controls)
                {
                    Console.SetCursorPosition(container.CaptionX, container.CaptionY);
                    Console.Write(container.CaptionText);
                    container.Control.Render();
                }
            }
        }

        public void SetFocus()
        {
            _controls.FirstOrDefault()?.Control?.SetFocus();
        }
    }
}