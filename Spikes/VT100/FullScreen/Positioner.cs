﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using VT100.FullScreen.Controls;

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

        private List<ControlContainer> _combinedControls;
        private List<ControlContainer> _controls;
        private List<ControlContainer> _buttons;

        public Positioner(int width, int height, CaptionAlignment captionAlignment, IEnumerable<ILayoutControl> controls, IFullScreenConsole console)
        {
            _console = console;
            Width = width;
            Height = height;
            CaptionAlignment = captionAlignment;

            Row = 1;
            Column = 1;

            var allControls = controls.Select(c => new ControlContainer(c)).ToList();
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
            Row += maxHeight;
            var column = Width;
            foreach (var container in _buttons.Select(b => b).Reverse()) 
            {
                container.CaptionText = $"{container.Caption.PadRight(maxCaption)}:";
                var requestedSize = container.Control.GetRequestedSize();
                column -= requestedSize.Width;
                var controlY = Row - requestedSize.Height;
                var controlX = column--;
                container.Control.Position(controlX, controlY, requestedSize.Width, requestedSize.Height);
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
    }
}