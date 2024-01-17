using TestConsoleLib.Testing;
using VT100.Attributes;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.Tests.Fakes;
using Xunit;

namespace VT100.Tests.Fullscreen
{
    public class TestStackLayout
    {
        #region Types for test

        [Screen]
        [Border(BorderType.Normal)]
        [Background(VtColour.Blue)]
        [InputBackground(VtColour.Yellow)]
        [StackLayout(StackDirection = StackDirection.Horizontal)]
        class LayoutForTest : ILayout
        {
            #region Implementation of ILayout

            public event LayoutUpdated LayoutUpdated;

            #endregion

            [TextBox("Real Name")]
            [InputForeground(VtColour.Black)]
            public string Name { get; set; }

            [TextBox("Nickname")]
            [InputBackground(VtColour.Red)]
            public string NickName { get; set; }

            [TextBox("Favourite Colour")]
            [InputBackground(VtColour.Red)]
            public string Colour { get; set; }
        }
        
        #endregion

        [Fact]
        public void ControlsArePositioned()
        {
            //Arrange
            var layout = new LayoutForTest() { Name = "Test", NickName = "Wolf", Colour = "Red" };
            var app = new FakeFullScreenApplication(layout, 50, 25);
    
            //Act
            app.Start();

            //Assert
            app.Console.GetDisplayReport().Verify();
        }
    }
}