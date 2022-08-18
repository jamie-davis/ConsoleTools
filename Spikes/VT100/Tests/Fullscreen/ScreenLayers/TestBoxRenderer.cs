using System.Collections.Generic;
using TestConsoleLib.Testing;
using VT100.FullScreen.ScreenLayers;
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
    }
}