using System.Collections.Generic;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    internal interface IRegionControl
    {
        ControlSet ComputePosition(ControlContainer controlContainer, int regionCol, int regionRow, int width,
            int height, Viewport containingViewport);

        IEnumerable<ILayoutControl> GetLayoutControls();
    }
}