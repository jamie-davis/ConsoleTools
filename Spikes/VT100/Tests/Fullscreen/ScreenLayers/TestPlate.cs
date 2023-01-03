using TestConsoleLib.Testing;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Tests.Utilities;
using Xunit;

namespace VT100.Tests.Fullscreen.ScreenLayers
{
    public class TestPlate
    {
        [Fact]
        public void PlateAcceptsText()
        {
            //Arrange
            var plate = new Plate(11,3);
            
            //Act
            plate.WriteText(3,1,"Hello");

            //Assert
            PlateDumpFormatter.Format(plate).Verify();
        }

        [Fact]
        public void PlateAcceptsColourFormat()
        {
            //Arrange
            var plate = new Plate(11,3);
            
            //Act
            plate.WriteText(3,1,"Hello", new DisplayFormat { Foreground = VtColour.Green });

            //Assert
            PlateDumpFormatter.Format(plate).Verify();
        }
    }
}