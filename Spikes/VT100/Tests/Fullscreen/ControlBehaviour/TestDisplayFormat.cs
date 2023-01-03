using System.Linq;
using FluentAssertions;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using Xunit;

namespace VT100.Tests.Fullscreen.ControlBehaviour
{
    public class TestDisplayFormat
    {
        [Fact]
        public void ApplyChangesForeground()
        {
            //Arrange
            DisplayFormat target;
            target.Foreground = VtColour.Red;
            target.Background = VtColour.NoColourChange;

            DisplayFormat source;
            source.Foreground = VtColour.Blue;
            source.Background = VtColour.NoColourChange;

            //Act
            target.Apply(source);

            //Assert
            target.Foreground.Should().Be(VtColour.Blue);
        }

        [Fact]
        public void ApplyChangesBackground()
        {
            //Arrange
            DisplayFormat target;
            target.Foreground = VtColour.NoColourChange;
            target.Background = VtColour.Red;

            DisplayFormat source;
            source.Background = VtColour.Blue;
            source.Foreground = VtColour.NoColourChange;

            //Act
            target.Apply(source);

            //Assert
            target.Background.Should().Be(VtColour.Blue);
        }

        [Fact]
        public void ApplyOfDefaultFormatDoesNotChangeForeground()
        {
            //Arrange
            DisplayFormat target;
            target.Foreground = VtColour.Red;
            target.Background = VtColour.NoColourChange;

            //Act
            target.Apply(default);

            //Assert
            target.Foreground.Should().Be(VtColour.Red);
        }

        [Fact]
        public void ApplyOfDefaultFormatDoesNotChangeBackground()
        {
            //Arrange
            DisplayFormat target;
            target.Foreground = VtColour.NoColourChange;
            target.Background = VtColour.Red;

            //Act
            target.Apply(default);

            //Assert
            target.Background.Should().Be(VtColour.Red);
        }

        [Fact]
        public void ApplyProcessesRangeInArray()
        {
            //Arrange
            var array = new DisplayFormat[4];
            for (var n = 0; n < array.Length; ++n)
            {
                array[n].Foreground = VtColour.Blue;
            }

            DisplayFormat source;
            source.Foreground = VtColour.Red;
            source.Background = VtColour.NoColourChange;

            //Act
            source.Apply(array, 1, 2);

            //Assert
            string.Join(" ", array.Select(a => a.Foreground.ToString())).Should().Be("Blue Red Red Blue");
        }
    }
}