using System.Collections.Generic;
using System.Linq;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.Attributes;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.Tests.TestUtilities;
using Xunit;

namespace VT100.Tests.Fullscreen
{
    public class TestLayoutControls
    {
        #region Types for test

        class LayoutFullscreenImpl : IFullScreenApplication
        {
            #region Implementation of IFullScreenApplication

            public void GotFocus(ILayoutControl focusControl)
            {
                throw new System.NotImplementedException();
            }

            public bool IsCursorModeInsert()
            {
                throw new System.NotImplementedException();
            }

            public void CloseScreen()
            {
                throw new System.NotImplementedException();
            }

            public void ReRender()
            {
                throw new System.NotImplementedException();
            }

            #endregion
        }

        [Screen]
        [Border(BorderType.Normal)]
        [Background(VtColour.Blue)]
        [InputBackground(VtColour.Yellow)]
        [StackLayout(StackDirection = StackDirection.Horizontal)]
        class Layout : ILayout
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

        [Screen]
        [Border(BorderType.Normal)]
        [Background(VtColour.Blue)]
        [InputBackground(VtColour.Yellow)]
        class NestedLayout : ILayout
        {
            public class NestedHeaderLayout
            {
                [Label("Nested label")]
                public string Header => "nested label value";

                [TextBox("Nested text box")]
                public string Value { get; set; } = "nested value";
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

        private static void ReportControlList(List<LayedOutControl> layoutControls, Output output)
        {
            var report = layoutControls.AsReport(rep => rep
                .AddColumn(l => l.Control.GetType().Name, cc => cc.Heading("Type"))
                .AddColumn(l => l.Control.Caption)
                .AddChild(l => l.PropertySettings, crep => crep.Title("Properties")
                    .AddColumn(p => p.Property)
                    .AddColumn(p => p.GetValueType().Name, cc => cc.Heading("Value Type"))
                    .AddColumn(p => p.GetValue(), cc => cc.Heading("Value"))
                ));
            output.FormatTable(report);
        }

        [Fact]
        public void LaidOutControlsAreExtracted()
        {
            //Arrange
            var app = new LayoutFullscreenImpl();
            var layout = new Layout();
            var rig = new ViewportTestRig(30, 20, 20, 10);

            //Act
            var layoutControls = LayoutControls.Extract(app, layout, rig.Vp).ToList();

            //Assert
            var output = new Output();
            ReportControlList(layoutControls, output);
            output.Report.Verify();
        }

        [Fact]
        public void NestedLayoutIsIncluded()
        {
            //Arrange
            var app = new LayoutFullscreenImpl();
            var layout = new NestedLayout();
            var rig = new ViewportTestRig(30, 20, 20, 10);

            //Act
            var layoutControls = LayoutControls.Extract(app, layout, rig.Vp).ToList();

            //Assert
            var output = new Output();
            ReportControlList(layoutControls, output);
            output.Report.Verify();
        }
    }
}