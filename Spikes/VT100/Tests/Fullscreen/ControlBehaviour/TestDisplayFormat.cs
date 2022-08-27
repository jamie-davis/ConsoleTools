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
        public void ApplyChangesColour()
        {
            //Arrange
            DisplayFormat target;
            target.Colour = VtColour.Red;

            DisplayFormat source;
            source.Colour = VtColour.Blue;

            //Act
            target.Apply(source);

            //Assert
            target.Colour.Should().Be(VtColour.Blue);
        }
        [Fact]
        public void ApplyOfDefaultFormatDoesNotChangeColour()
        {
            //Arrange
            DisplayFormat target;
            target.Colour = VtColour.Red;

            //Act
            target.Apply(default);

            //Assert
            target.Colour.Should().Be(VtColour.Red);
        }

        [Fact]
        public void ApplyProcessesRangeInArray()
        {
            //Arrange
            var array = new DisplayFormat[4];
            for (var n = 0; n < array.Length; ++n)
            {
                array[n].Colour = VtColour.Blue;
            }

            DisplayFormat source;
            source.Colour = VtColour.Red;

            //Act
            source.Apply(array, 1, 2);

            //Assert
            string.Join(" ", array.Select(a => a.Colour.ToString())).Should().Be("Blue Red Red Blue");
        }
    }
}