using System;
using System.Collections.Generic;
using System.Linq;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen.Controls
{
    /// <summary>
    /// This control can only be created manually, and is not related to an attribute to allow an instance to be created
    /// as part of an application or region definition class. 
    /// </summary>
    internal class ButtonBoxControl : ILayoutControl, IRegionControl
    {
        private List<ControlContainer> _controls;
        private int _column;
        private int _row;
        private int _width;
        private int _height;

        public ButtonBoxControl(List<ControlContainer> controls)
        {
            _controls = controls;
        }

        #region Implementation of ILayoutControl

        public int Column => _column;

        public int Row => _row;

        public int Width => _width;

        public int Height => _height;

        public IEnumerable<BoxRegion> BoxRegions { get; }

        public IEnumerable<ControlContainer> ContainedControls => _controls.ToList();
        public void PropertyBind(IFullScreenApplication app, object layout, Func<object, object> getter, Action<object, object> setter)
        {
            throw new NotImplementedException();
        }

        public void MethodBind(IFullScreenApplication app, object layout, Func<object, bool> method)
        {
            throw new NotImplementedException();
        }

        public string Caption { get; }
        public void Render(IFullScreenConsole console)
        {
            throw new NotImplementedException();
        }

        public void Position(int column, int row, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetFocus(IFullScreenConsole console)
        {
            _controls.FirstOrDefault()?.Control.SetFocus(console);
        }

        public void Accept(IFullScreenConsole console, ControlSequence next)
        {
            throw new NotImplementedException();
        }

        public BorderBorderStyle BorderBorderStyle { get; }
        public void Refresh(IFullScreenConsole console)
        {
            foreach (var controlContainer in _controls)
            {
                controlContainer.Control.Refresh(console);
            }
        }

        #endregion

        #region Implementation of IRegionControl

        public ControlSet ComputePosition(ControlContainer controlContainer, int regionCol, int regionRow, int width, int height,
            Viewport containingViewport)
        {
            var space = _controls.Select(b => b.Control.GetRequestedSize()).ToList();
            var maxHeight = space.Max(m => (int?)m.Height) ?? 0;
            var fullWidth = (space.Sum(m => (int?)m.Width) ?? 0) + space.Count - 1;
            var buttonColumn = ((width - fullWidth) / 2) + regionCol;
            var row = regionRow + maxHeight;
            foreach (var container in _controls)
            {
                var requestedSize = container.Control.GetRequestedSize();
                var controlY = row - requestedSize.Height;
                var controlX = buttonColumn;
                buttonColumn += requestedSize.Width + 1;
                container.Control.Position(controlX, controlY, requestedSize.Width, requestedSize.Height);
            }

            var boxRegions = _controls
                .Where(c => c.LayoutProperties.HasBorder())
                .Select(c => c.RenderBorder())
                .Concat(_controls.SelectMany(c => c.Control.BoxRegions))
                .ToList();

            var viewports = new List<Viewport> { containingViewport };

            _height = maxHeight;
            _width = fullWidth;
            _column = buttonColumn;
            _row = regionRow;
            
            return new ControlSet(_controls.ToList(), boxRegions, viewports, width, maxHeight);
        }

        public IEnumerable<ILayoutControl> GetLayoutControls()
        {
            return _controls.Select(c => c.Control);
        }

        #endregion
    }
}