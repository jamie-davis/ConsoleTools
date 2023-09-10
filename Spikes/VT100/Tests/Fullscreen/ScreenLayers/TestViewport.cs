using System;
using System.Text;
using FluentAssertions;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.FullScreen.ScreenLayers;
using VT100.Tests.Fakes;
using VT100.Tests.TestUtilities;
using Xunit;

namespace VT100.Tests.Fullscreen.ScreenLayers
{
    public class TestViewport
    {
        [Fact]
        public void ViewportCanBeRendered()
        {
            //Arrange
            var rig = new ViewportTestRig(30, 20, 20, 10);

            //Act
            rig.Vp.Render(0, 0, null);
            
            //Assert
            rig.GetDisplayReport().Verify();
        }

        [Fact]
        public void ViewportCanBeMoved()
        {
            //Arrange
            var sb = new StringBuilder();
            var rig = new ViewportTestRig(12, 7, 10, 5);

            //Act
            for (var i = 0; i < 5; i++)
            {
                rig.Vp.Render(0, 0, null);
                sb.AppendLine($"Origin: {rig.Vp.FirstContainedVisibleColumn}, {rig.Vp.FirstContainedVisibleRow}");
                sb.Append(rig.GetDisplayReport(DisplayReportOptions.NoDiagnostics));
                sb.AppendLine();
                
                rig.Vp.SetOrigin(rig.Vp.FirstContainedVisibleColumn +  2, rig.Vp.FirstContainedVisibleRow + 1);
            }
            
            //Assert
            sb.ToString().Verify();
        }
        
        [Fact]
        public void ViewportCanBeMovedOutOfAvailableDataArea()
        {
            //Arrange
            var sb = new StringBuilder();
            var rig = new ViewportTestRig(12, 7, 10, 5);

            //Act
            for (var i = 0; i < 6; i++)
            {
                rig.Vp.Render(0, 0, null);
                sb.AppendLine($"Origin: {rig.Vp.FirstContainedVisibleColumn}, {rig.Vp.FirstContainedVisibleRow}");
                sb.Append(rig.GetDisplayReport(DisplayReportOptions.NoDiagnostics));
                sb.AppendLine();
                
                rig.Vp.SetOrigin(rig.Vp.FirstContainedVisibleColumn +  5, rig.Vp.FirstContainedVisibleRow + 3);
            }
            
            //Assert
            sb.ToString().Verify();
        }
        
        [Fact]
        public void CharacterCanBeBroughtIntoViewFromTopLeft()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(5, 10);
            
            //Act
            rig.Vp.BringIntoView(1, 2);

            //Assert
            rig.GetOrigin().Should().Be((1, 2));
        }
        
        [Fact]
        public void CharacterCanBeBroughtIntoViewFromTopRight()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(5, 10);
            
            //Act
            rig.Vp.BringIntoView(16, 2);

            //Assert
            rig.GetOrigin().Should().Be((7, 2));
        }
        
        [Fact]
        public void CharacterCanBeBroughtIntoViewFromBottomRight()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(0, 0);
            
            //Act
            rig.Vp.BringIntoView(13, 14);

            //Assert
            rig.GetOrigin().Should().Be((4, 10));
        }
        
        [Fact]
        public void CharacterCanBeBroughtIntoViewFromBottomLeft()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(10, 0);
            
            //Act
            rig.Vp.BringIntoView(3, 14);

            //Assert
            rig.GetOrigin().Should().Be((3, 10));
        }
        
        [Fact]
        public void CharacterAlreadyInViewBroughtIntoViewLeavesOrigin()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(0, 11);
            
            //Act
            rig.Vp.BringIntoView(3, 14);

            //Assert
            rig.GetOrigin().Should().Be((0, 11));
        }
        
        [Fact]
        public void RectangleCanBeBroughtIntoViewFromTopLeft()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(5, 10);
            var rectangle = new Rectangle((2, 2), (3, 3), (1, 1));
            
            //Act
            rig.Vp.BringIntoView(rectangle);

            //Assert
            rig.GetOrigin().Should().Be((2, 2));
        }
        
        [Fact]
        public void RectangleCanBeBroughtIntoViewFromTopRight()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(1, 10);
            var rectangle = new Rectangle((15, 4), (3, 3), (1, 1));
            
            //Act
            rig.Vp.BringIntoView(rectangle);

            //Assert
            rig.GetOrigin().Should().Be((8, 4));
        }
        
        [Fact]
        public void RectangleCanBeBroughtIntoViewFromBottomLeft()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(10, 3);
            var rectangle = new Rectangle((1, 14), (3, 3), (1, 1));
            
            //Act
            rig.Vp.BringIntoView(rectangle);

            //Assert
            rig.GetOrigin().Should().Be((1, 12));
        }
        
        [Fact]
        public void RectangleCanBeBroughtIntoViewFromBottomRight()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(1, 3);
            var rectangle = new Rectangle((17, 14), (3, 3), (1, 1));
            
            //Act
            rig.Vp.BringIntoView(rectangle);

            //Assert
            rig.GetOrigin().Should().Be((10, 12));
        }
        
        [Fact]
        public void OversizedRectangleCanBeBroughtIntoView()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(1, 3);
            var rectangle = new Rectangle((3, 2), (13, 13), (10, 9));
            
            //Act
            rig.Vp.BringIntoView(rectangle);

            //Assert
            rig.GetOrigin().Should().Be((5, 7));
        }
        
        [Fact]
        public void RectangleIsAdjustedForAvailableSpace()
        {
            //Arrange
            var output = new Output();
            void CheckRectangle(Rectangle rectangle, string message)
            {
                output.WrapLine(message);
                output.WriteLine();
                output.WrapLine("Raw contained view:");
                var rig = new ViewportTestRig(20, 10, 10, 5, RigOption.Empty);
                rig.Vp.SetOrigin(1, 3);

                var fillRow = new string('.', rectangle.Width);
                var numberString = "012345678901234567890123456789";
                for (int i = rectangle.Top; i < rectangle.Top + rectangle.Height; i++)
                {
                    rig.Plate.WriteText(rectangle.Left, i, fillRow);
                    rig.Plate.WriteText(rectangle.KeyCol, i, numberString.Substring(i - rectangle.Top, 1));
                }

                rig.Plate.WriteText(rectangle.Left, rectangle.KeyRow, numberString.Substring(0, rectangle.Width));
                rig.Plate.WriteText(rectangle.KeyCol, rectangle.KeyRow, "@");

                rig.Vp.BringIntoView(rectangle);
                rig.Vp.Render(0, 0, null);

                output.WriteLine(rig.Plate.Dump());
            
                output.WrapLine("Viewport Rendered:");
                output.WriteLine(rig.GetDisplayReport());
            }

            //Act
            CheckRectangle(new Rectangle((3, 3), (15, 10), (9, 9)), "Key Col is centred in reduced rectangle:");
            CheckRectangle(new Rectangle((5, 3), (10, 10), (5, 9)), "Key Col on left edge is left edge of adjusted rectangle:");
            CheckRectangle(new Rectangle((5, 3), (10, 10), (14, 9)), "Key Col on right edge is left edge of adjusted rectangle:");
            CheckRectangle(new Rectangle((5, 3), (10, 10), (9, 3)), "Key Col on top edge is top edge of adjusted rectangle:");
            CheckRectangle(new Rectangle((5, 3), (10, 10), (9, 12)), "Key Col on bottom edge is bottom edge of adjusted rectangle:");

            //Assert
            output.Report.Verify();
        }
        
        [Fact]
        public void OversizedRectangleBroughtIntoViewReturnsParentViewRectangle()
        {
            //Arrange
            var rig = new ViewportTestRig(12, 7, 10, 5);
            rig.Vp.SetOrigin(1, 3);
            var rectangle = new Rectangle((3, 2), (13, 13), (10, 9));
            
            //Act
            var result = rig.Vp.BringIntoView(rectangle);

            //Assert
            var vpRect = rig.GetRectangle(6, 3);
            result.Describe().Should().Be(vpRect.Describe());
        }
        
        [Fact]
        public void ParentRectangleReturnedAfterBringIntoView()
        {
            //Arrange
            var output = new Output();
            output.WrapLine("Note that when the test rectangle is bigger than the viewport, its border may not be visible, while the console rectangle will always have a visible border in the diagrams below.");
            var tests = new[]
            {
                new { Origin = (1, 3), Rectangle = new Rectangle((3, 2), (3, 3), (4, 3))},
                new { Origin = (10, 3), Rectangle = new Rectangle((1, 11), (3, 3), (2, 12))},
                new { Origin = (1, 3), Rectangle = new Rectangle((3, 2), (13, 13), (10, 9))},
                new { Origin = (1, 3), Rectangle = new Rectangle((3, 2), (13, 5), (10, 4))},
            };
            
            //Act
            foreach (var test in tests)
            {
                Test(test.Origin, test.Rectangle);
            }

            void Test((int col, int row) origin, Rectangle rectangle)
            {
                var rig = new ViewportTestRig(12, 7, 10, 5, RigOption.Empty);
                rig.Vp.SetOrigin(origin.col, origin.row);
                RenderRectangle(rectangle, rig, '+');
                rig.Vp.Render(0, 0, null);

                output.WrapLine("---------------------------");
                output.WrapLine($"Before bringing rectangle {rectangle.Describe()} into view:");
                output.WrapLine($"Viewport origin = ({origin.col},{origin.row})");
                output.WrapLine(rig.GetDisplayReport(DisplayReportOptions.NoDiagnostics));

                //Act
                var result = rig.Vp.BringIntoView(rectangle);

                //Assert
                rig.Vp.Render(0, 0, null);
                output.WrapLine($"After bringing rectangle {rectangle.Describe()} into view:");
                output.WrapLine($"Viewport origin = ({rig.Vp.FirstContainedVisibleColumn},{rig.Vp.FirstContainedVisibleRow})");
                output.WrapLine(rig.GetDisplayReport(DisplayReportOptions.NoDiagnostics));
                output.WrapLine($"Returned console rectangle overlayed:");
                RenderRectangle(result, rig.Console, '=');
                output.WrapLine(rig.Console.GetDisplayReport(DisplayReportOptions.NoDiagnostics));
                output.WriteLine();
            }
            
            //Assert
            output.Report.Verify();
        }

        private void RenderRectangle(Rectangle rectangle, ViewportTestRig rig, char renderChar)
        {
            Action<int, int, string> write = (col, row, text) =>
            {
                if (col < 0 || row < 0 || col >= rig.Plate.Width || row >= rig.Plate.Height) return;
                if (text.Length + col >= rig.Plate.Width) text = text.Substring(0, rig.Plate.Width - col - 1);
                rig.Plate.WriteText(col, row, text);
            };

            RenderRectangleImpl(rectangle, renderChar, write);
        }

        private void RenderRectangle(Rectangle rectangle, FakeFullScreenConsole console, char renderChar)
        {
            Action<int, int, string> write = (col, row, text) =>
            {
                if (col < 0 || row < 0 || col >= console.WindowWidth || row >= console.WindowHeight) return;
                console.SetCursorPosition(col, row);
                if (text.Length + col >= console.WindowWidth) text = text.Substring(0, console.WindowWidth - col - 1);
                console.Write(text);
            };

            RenderRectangleImpl(rectangle, renderChar, write);
        }

        private static void RenderRectangleImpl(Rectangle rectangle, char renderChar, Action<int, int, string> write)
        {
            write(rectangle.Left, rectangle.Top, new string(renderChar, rectangle.Width));
            write(rectangle.Left, rectangle.Top + rectangle.Height - 1, new string(renderChar, rectangle.Width));
            for (int i = 1; i < rectangle.Height - 1; i++)
            {
                write(rectangle.Left, rectangle.Top + i,
                    $"{renderChar}{new string(' ', rectangle.Width - 2)}{renderChar}");
            }

            write(rectangle.KeyCol, rectangle.KeyRow, renderChar.ToString());
        }
    }
}