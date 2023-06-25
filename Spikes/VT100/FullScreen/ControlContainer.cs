using System.Collections.ObjectModel;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.Controls;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    internal class ControlContainer
    {
        public ReadOnlyCollection<PropertySetting> PropertySettings { get; }
        private readonly ILayoutControl _control;

        public string CaptionText => _control.Caption ?? string.Empty;
        public ControlContainer(ILayoutControl control, ReadOnlyCollection<PropertySetting> propertySettings)
        {
            PropertySettings = propertySettings;
            _control = control;
            LabelControl = new LabelControl();
            LayoutProperties = new LayoutProperties();
            ControlPropertySetter.Set(LayoutProperties, propertySettings);
        }
            
        public LabelControl LabelControl { get; }
        public LayoutProperties LayoutProperties { get; }

        public ILayoutControl Control => _control;

        public int Column
        {
            get
            {
                var column = LabelControl.Width > 0 ? LabelControl.Column : Control?.Column ?? 0;
                return LayoutProperties.HasBorder() ? column - 1 : column;
            }
        }

        public int Row
        {
            get
            {
                var row = LabelControl.Height > 0 ? LabelControl.Row : Control?.Row ?? 0;
                return LayoutProperties.HasBorder() ? row - 1 : row;
            }
        }

        public int Width
        {
            get
            {
                var controlWidth = Control.Column - LabelControl.Column + Control.Width;
                return LayoutProperties.HasBorder() ? controlWidth + 2 : controlWidth;
            }
        }
        public int Height => LayoutProperties.HasBorder() ? Control.Height + 2 : Control.Height;

        public BoxRegion RenderBorder()
        {
            return new BoxRegion(Column, Row, Width, Height, LineWeight.Light);
        }
    }
}