using TestConsoleLib.Testing;
using VT100.FullScreen.ScreenLayers;
using VT100.Tests.Utilities;
using Xunit;

namespace VT100.Tests.Fullscreen.ScreenLayers
{
    public class TestBoxMapMaker
    {
        [Fact]
        public void BoxRegionIsMapped()
        {
            //Arrange
            var region = new BoxRegion(1, 1, 5, 5);
            
            //Act
            var map = BoxMapMaker.Map(new[] { region }, 10, 10);

            //Assert
            var plate = new Plate(10, 10);
            BoxRenderer.RenderMapToPlate(map, plate);
            PlateDumpFormatter.Format(plate).Verify();
        }

        [Fact]
        public void BoxRegionThatExceedsRightEdgeIsTrimmed()
        {
            //Arrange
            var region = new BoxRegion(7, 1, 5, 5);
            
            //Act
            var map = BoxMapMaker.Map(new[] { region }, 10, 10);

            //Assert
            var plate = new Plate(10, 10);
            BoxRenderer.RenderMapToPlate(map, plate);
            PlateDumpFormatter.Format(plate).Verify();
        }

        [Fact]
        public void BoxRegionThatExceedsBottomEdgeIsTrimmed()
        {
            //Arrange
            var region = new BoxRegion(1, 7, 5, 5);
            
            //Act
            var map = BoxMapMaker.Map(new[] { region }, 10, 10);

            //Assert
            var plate = new Plate(10, 10);
            BoxRenderer.RenderMapToPlate(map, plate);
            PlateDumpFormatter.Format(plate).Verify();
        }

        [Fact]
        public void BoxRegionThatExceedsTopEdgeIsTrimmed()
        {
            //Arrange
            var region = new BoxRegion(1, -2, 5, 5);
            
            //Act
            var map = BoxMapMaker.Map(new[] { region }, 10, 10);

            //Assert
            var plate = new Plate(10, 10);
            BoxRenderer.RenderMapToPlate(map, plate);
            PlateDumpFormatter.Format(plate).Verify();
        }

        [Fact]
        public void BoxRegionOnlyNeedsOneCharacterInView()
        {
            //Arrange
            var regions = new []
            {
                new BoxRegion(-4, -4, 5, 5),
                new BoxRegion(9, 9, 5, 5),
                new BoxRegion(9, -4, 5, 5),
                new BoxRegion(-4, 9, 5, 5)
            };
            
            //Act
            var map = BoxMapMaker.Map(regions, 10, 10);

            //Assert
            var plate = new Plate(10, 10);
            BoxRenderer.RenderMapToPlate(map, plate);
            PlateDumpFormatter.Format(plate).Verify();
        }

        [Fact]
        public void BoxRegionThatIsOffScreenToBottomAndRightIsSkipped()
        {
            //Arrange
            var region = new BoxRegion(11, 11, 5, 5);
            
            //Act
            var map = BoxMapMaker.Map(new[] { region }, 10, 10);

            //Assert
            var plate = new Plate(10, 10);
            BoxRenderer.RenderMapToPlate(map, plate);
            PlateDumpFormatter.Format(plate).Verify();
        }

        [Fact]
        public void BoxRegionsEdgesCanOverlap()
        {
            //Arrange
            var regions = new []
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
            
            //Act
            var map = BoxMapMaker.Map(regions, 10, 10);

            //Assert
            var plate = new Plate(10, 10);
            BoxRenderer.RenderMapToPlate(map, plate);
            PlateDumpFormatter.Format(plate).Verify();
        }
    }
}