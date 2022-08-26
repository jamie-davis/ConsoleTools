using System.Collections.Generic;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen.ScreenLayers
{
    internal static class BoxRenderer
    {
        internal static void RenderToPlate(IEnumerable<BoxRegion> regions, Plate plate)
        {
            var map = BoxMapMaker.Map(regions, plate.Width, plate.Height);
            RenderMapToPlate(map, plate);
        }

        internal static void RenderMapToPlate(BoxMap map, Plate plate)
        {
            for (int y = 0; y < plate.Height; y++)
            {
                for (int x = 0; x < plate.Width; x++)
                {
                    var mapItem = map.GetAt(x, y);
                    if (mapItem.Class != null)
                    {
                        plate.WriteText(x, y, ((char)mapItem.Class.Source).ToString());
                    }
                }
            }
        }
    }
}