using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VT100.Attributes;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen.Controls
{
    [Control(typeof(RegionAttribute))]

    internal class RegionControl : ILayoutControl, IRegionControl
    {
        private int _column;
        private int _row;
        private int _width;
        private int _height;
        private List<BoxRegion> _boxRegions = new List<BoxRegion>();
        private string _caption;
        private BorderBorderStyle _borderBorderStyle;
        private object _dataContainer;
        private Func<object, object> _getter;
        private Action<object, object> _setter;

        private object _value;
        private IFullScreenApplication _app;
        private CursorController _cursorControl;

        #region Implementation of ILayoutControl

        public int Column => _column;

        public int Row => _row;

        public int Width => _width;

        public int Height => _height;

        public IEnumerable<BoxRegion> BoxRegions => _boxRegions;

        public void PropertyBind(IFullScreenApplication app, object layout, Func<object, object> getter,
            Action<object, object> setter)
        {
            _app = app;
            _dataContainer = layout;
            _getter = getter;
            _setter = setter;
            RefreshValue();
        }

        private void RefreshValue()
        {
            var value = _getter(_dataContainer);
            if (ReferenceEquals(value, _value)) return;
            
            _value = value;
            //Analyse the value for controls etc
        }

        public void MethodBind(IFullScreenApplication app, object layout, Func<object, bool> method)
        {
            //Not possible for a region
        }

        public string Caption => _caption;

        public void Render(IFullScreenConsole console)
        {
            throw new NotImplementedException();
        }

        public void Position(int column, int row, int width, int height)
        {
            Debug.WriteLine($"Position region @{column},{row} {width}x{height}");
        }

        public void SetFocus(IFullScreenConsole console)
        {
            //Not possible for a region
        }

        public void Accept(IFullScreenConsole console, ControlSequence next)
        {
            //Not possible for a region
        }

        public BorderBorderStyle BorderBorderStyle => _borderBorderStyle;

        public void Refresh(IFullScreenConsole console)
        {
        }

        #endregion

        #region Implementation of IRegionControl

        public ControlSet ComputePosition(ControlContainer controlContainer, int regionCol, int regionRow, int width,
            int height)
        {
            var layoutControls = LayoutControls.Extract(_app, _value).ToList();
            var props = ControlPropertyExtractor.Extract(_value?.GetType());
            var regionProps = new LayoutProperties();
            ControlPropertySetter.Set(regionProps, props);
            var controlSet = Positioner.Position(regionCol, regionRow, width, height, CaptionAlignment.Left, layoutControls, regionProps);
            _height = controlSet.TotalHeight;
            _width = controlSet.TotalWidth;
            _column = regionCol;
            _row = regionRow;
            return controlSet;
        }

        #endregion
    }
}