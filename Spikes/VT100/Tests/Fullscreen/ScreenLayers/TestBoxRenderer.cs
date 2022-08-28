using System.Collections.Generic;
using TestConsoleLib.Testing;
using VT100.FullScreen.ScreenLayers;
using VT100.Tests.Fakes;
using VT100.Tests.Utilities;
using Xunit;

namespace VT100.Tests.Fullscreen.ScreenLayers
{
    public class TestBoxRenderer
    {
        [Fact]
        public void RenderToPlateDrawsSimpleBox()
        {
            //Arrange
            var plate = new Plate(15, 5);
            var boxes = new List<BoxRegion> { new BoxRegion(2, 1, 12, 2) };

            //Act
            BoxRenderer.RenderToPlate(boxes, plate);

            //Assert
            PlateDumpFormatter.Format(plate).Verify();
        }
        
        [Fact]
        public void BoxRegionsCanBeRenderedDirectToConsole()
        {
            //Arrange
            var regions = new[]
            {
                new BoxRegion(0, 0, 3, 3),
                new BoxRegion(2, 0, 3, 3),
                new BoxRegion(4, 0, 3, 3),
                new BoxRegion(6, 0, 3, 3),
                new BoxRegion(8, 0, 3, 3),
                new BoxRegion(1, 2, 3, 3),
                new BoxRegion(3, 2, 3, 3),
                new BoxRegion(5, 2, 3, 3),
                new BoxRegion(7, 2, 3, 3),
                new BoxRegion(1, 4, 3, 3),
                new BoxRegion(3, 4, 3, 3),
                new BoxRegion(5, 4, 3, 3),
                new BoxRegion(7, 4, 3, 3),
            };
            var map = BoxMapMaker.Map(regions, 10, 10);
            var console = new FakeFullScreenConsole(10, 10);

            //Act
            BoxRenderer.RenderMapToConsole(map, console);

            //Assert
            console.GetDisplayReport().Verify();
        }
    }
}