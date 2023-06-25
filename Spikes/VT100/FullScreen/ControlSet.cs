using System.Collections.Generic;
using System.Linq;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.Controls;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    internal class ControlSet
    {
        public int TotalWidth { get; }
        public int TotalHeight { get; }
        private List<BoxRegion> _regions;
        private List<ControlContainer> _combinedControls;
        private List<ControlContainer> _controls;
        private List<ControlContainer> _buttons;
        
        public FocusController FocusController { get; private set; }

        public ControlSet(IEnumerable<ControlContainer> controls, IEnumerable<BoxRegion> boxRegions, int totalWidth, int totalHeight)
        {
            TotalWidth = totalWidth;
            TotalHeight = totalHeight;
            _regions = boxRegions.ToList();
            _combinedControls = controls.ToList();
            _controls = _combinedControls.Where(c => c.Control is not ButtonControl).ToList();
            _buttons = _combinedControls.Where(c => c.Control is ButtonControl).ToList();
            FocusController = new FocusController(_combinedControls.Select(c => c.Control));
        }
        
        public void Render(List<PropertySetting> settings, IFullScreenConsole console)
        {
            using (new CursorHider())
            {
                var boxRegions = _regions.Concat(_combinedControls.SelectMany(c => c.Control.BoxRegions));
                var map = BoxMapMaker.Map(boxRegions, console.WindowWidth, console.WindowHeight);
                DisplayFormat format = new ()
                {
                    Foreground = VtColour.NoColourChange,
                    Background = VtColour.NoColourChange,
                };
                BoxRenderer.RenderMapToConsole(map, console, format);
                
                foreach (var container in _controls)
                {
                    ControlSettingsUpdater.Update(container.Control, settings, container.PropertySettings);
                    container.LabelControl.Refresh(console);
                    container.Control.Refresh(console);
                }
                
                foreach (var container in _buttons)
                {
                    ControlSettingsUpdater.Update(container.Control, settings, container.PropertySettings);
                    container.Control.Refresh(console);
                }
            }
        }
        public void ReRender(IFullScreenConsole console)
        {
            foreach (var control in _combinedControls)
            {
                control.Control.Refresh(console);
            }
        }

        public IEnumerable<ControlContainer> ExportControls()
        {
            return _controls.ToList();
        }

        public IEnumerable<BoxRegion> ExportBoxRegions()
        {
            return _regions.ToList();
        }
    }
}