using FluentAssertions;
using VT100.FullScreen.ScreenLayers;
using Xunit;

namespace VT100.Tests.Fullscreen.ScreenLayers
{
    public class TestRectangleReducer
    {
        [Fact]
        public void TooWideRectangleIsReducedInWidth()
        {
            //Arrange
            var rectangle = new Rectangle((0,0),(25,7),(10,4));

            //Act
            var result = RectangleReducer.Reduce(rectangle, 10, 5);
            
            //Assert
            result.Width.Should().Be(10);
        }

        [Fact]
        public void TooTallRectangleIsReducedInHeight()
        {
            //Arrange
            var rectangle = new Rectangle((0,0),(25,7),(10,4));

            //Act
            var result = RectangleReducer.Reduce(rectangle, 10, 5);
            
            //Assert
            result.Height.Should().Be(5);
        }

        [Fact]
        public void ReducedRectangleKeepsKeyPointHorizontallyCentral()
        {
            //Arrange
            var rectangle = new Rectangle((0,0),(25,7),(9,4));

            //Act
            var result = RectangleReducer.Reduce(rectangle, 7, 5);
            
            //Assert
            result.Left.Should().Be(6);
        }
        
        [Fact]
        public void ReducedRectangleKeepsKeyPointVerticallyCentral()
        {
            //Arrange
            var rectangle = new Rectangle((0,0),(25,7),(9,4));

            //Act
            var result = RectangleReducer.Reduce(rectangle, 7, 5);
            
            //Assert
            result.Top.Should().Be(2);
        }

        [Fact]
        public void ReducedRectangleRetainsKeyCol()
        {
            //Arrange
            var rectangle = new Rectangle((0,0),(25,7),(9,4));

            //Act
            var result = RectangleReducer.Reduce(rectangle, 7, 5);
            
            //Assert
            result.KeyCol.Should().Be(9);
        }
        
        [Fact]
        public void ReducedRectangleRetainsKeyRow()
        {
            //Arrange
            var rectangle = new Rectangle((0,0),(25,7),(9,4));

            //Act
            var result = RectangleReducer.Reduce(rectangle, 7, 5);
            
            //Assert
            result.KeyRow.Should().Be(4);
        }
        
        [Fact]
        public void OriginalRectangleIsIsReturnedIfNoChangesAreNeeded()
        {
            //Arrange
            var rectangle = new Rectangle((0,0),(25,7),(9,4));

            //Act
            var result = RectangleReducer.Reduce(rectangle, 50, 50);
            
            //Assert
            result.Should().BeSameAs(rectangle);
        }
        
        [Fact]
        public void KeyPointOnBottomOfRectIsRetained()
        {
            //Arrange
            var rectangle = new Rectangle((0,0),(25,7),(9,6));

            //Act
            var result = RectangleReducer.Reduce(rectangle, 7, 5);
            
            //Assert
            result.Top.Should().Be(rectangle.KeyRow - result.Height + 1);
        }
    }
}