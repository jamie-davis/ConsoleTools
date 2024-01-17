using System.Collections.Generic;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    internal class PositioningOutcome
    {
        public PositioningOutcome(List<BoxRegion> boxRegions, List<ControlContainer> controls, List<Viewport> viewports, int totalWidth, int totalHeight)
        {
            BoxRegions = boxRegions;
            Controls = controls;
            Viewports = viewports;
            TotalWidth = totalWidth;
            TotalHeight = totalHeight;
        }

        public List<BoxRegion> BoxRegions { get; }
        public List<ControlContainer> Controls { get; }
        public List<Viewport> Viewports { get; }
        public int TotalWidth { get; }
        public int TotalHeight { get; }
    }
}