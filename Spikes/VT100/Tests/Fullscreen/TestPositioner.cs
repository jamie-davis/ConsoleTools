using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.Attributes;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
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
        class LayoutForTest : ILayout
        {
            public class NestedHeaderLayout
            {
                [Label("Nested label")] public string Header => "nested label value";

                [TextBox("Nested text box")] public string Value { get; set; } = "nested value";
            }

            [Region]
            [Border(BorderType.Normal)]
            [Background(VtColour.Black)]
            [Foreground(VtColour.BrightCyan)]
            public NestedHeaderLayout Header { get; set; } = new();

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

            [TextBox("Favourite Colour")]
            [InputBackground(VtColour.Red)]
            public string Colour { get; set; }

            [Button("Default")]
            public void SetDefaults()
            {
                Name = "Default name";
                NickName = "Defaulty";
            }

            [Button("OK", ExitMode.ExitOnSuccess)]
            public bool Ok()
            {
                return true;
            }
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

        [Fact]
        public void FocusIsInCorrectOrder()
        {
            //Arrange
            var layout = new LayoutForTest() { Name = "Test", NickName = "Wolf", Colour = "Red" };
            var app = new FakeFullScreenApplication(layout, 50, 25);

            //Act
            app.Start();

            //Assert
            var output = new Output();
            var displayReport = app.Console.GetDisplayReport();
            var focusChain = app.GetFocusChain();
            output.FormatTable(new[] { new { InitialDisplay = displayReport, FocusChain = focusChain } },
                ReportFormattingOptions.UnlimitedBuffer);
            output.Report.Verify();

        }
    }
}