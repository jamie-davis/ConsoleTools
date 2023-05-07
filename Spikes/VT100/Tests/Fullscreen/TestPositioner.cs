using System.Linq;
using TestConsoleLib.Testing;
using VT100.Attributes;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Tests.Fakes;
using Xunit;

namespace VT100.Tests.Fullscreen
{
    public class TestPositioner
    {
        #region Types for test

        [Screen]
        [Border(BorderType.Normal)]
        [Background(VtColour.Blue)]
        [InputBackground(VtColour.Yellow)]
        class Layout : ILayout
        {
            #region Implementation of ILayout

            public event LayoutUpdated LayoutUpdated;

            #endregion

            [TextBox("Real Name")]
            [Border(BorderType.Normal)]
            [InputForeground(VtColour.Black)]
            public string Name { get; set; }

            [TextBox("Nickname")]
            [InputBackground(VtColour.Red)]
            public string NickName { get; set; }

            [Button("Default")]
            public void SetDefaults()
            {
                Name = "Default name";
                NickName = "Defaulty";
            }
            
            [Button("OK", ExitMode.ExitOnSuccess)]
            public bool OK()
            {
                return true;
            }
        }


        #endregion
        
        [Fact]
        public void ControlsArePositioned()
        {
            //Arrange
            var layout = new Layout() { Name = "Test", NickName = "Wolf" };
            var app = new FakeFullScreenApplication(layout, 50, 20);
    
            //Act
            app.Start();

            //Assert
            app.Console.GetDisplayReport().Verify();
        }
    }
}