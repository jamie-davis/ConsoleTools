using System.Linq;
using System.Text;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.FullScreen.ScreenLayers;
using VT100.Tests.Fakes;
using Xunit;

namespace VT100.Tests.Fullscreen.ScreenLayers
{
    public class TestViewportStack
    {
        [Fact]
        public void ACharacterIsBroughtIntoViewInTheRootViewport()
        {
            //Arrange
            var console = new FakeFullScreenConsole(20, 20);
            var plate = new Plate(50, 50);
            var stack = new PlateStack(plate);
            var viewportStack = new ViewportStack(console, stack);
            var targetCol = 25;
            var targetRow = 25;
            plate.WriteText(targetCol, targetRow, "Here");
            
            //Act
            viewportStack.BringIntoView(viewportStack.RootViewport, targetCol, targetRow);

            //Assert
            viewportStack.Render();
            console.GetDisplayReport().Verify();
        }

        [Fact]
        public void ARectangleIsBroughtIntoViewInTheRootViewport()
        {
            //Arrange
            var console = new FakeFullScreenConsole(20, 20);
            var plate = new Plate(50, 50);
            var stack = new PlateStack(plate);
            var viewportStack = new ViewportStack(console, stack);
            var targetCol = 25;
            var targetRow = 25;
            plate.WriteText(targetCol, targetRow, "Here");

            var targetRect = new Rectangle((targetCol, targetRow - 1), (4, 3), (targetCol, targetRow));
            
            //Act
            viewportStack.BringIntoView(viewportStack.RootViewport, targetRect);

            //Assert
            viewportStack.Render();
            console.GetDisplayReport().Verify();
        }

        [Fact]
        public void TreeOfViewportsAreRendered()
        {
            //Arrange
            var console = new FakeFullScreenConsole(50, 20);
            var (stack, _) = MakeFullStack(console);

            //Act
            stack.Render();

            //Assert
            console.GetDisplayReport().Verify();
        }

        [Fact]
        public void ContainedViewportCharacterIsBroughtIntoView()
        {
            //Arrange
            var console = new FakeFullScreenConsole(50, 20);
            var (stack, viewports) = MakeFullStack(console);

            var outer = viewports[2];
            var outerPlate = outer.Plates[0];
            var charCol = outerPlate.Width - 1;
            var charRow = outerPlate.Height - 1;
            outerPlate.WriteText(charCol, charRow, "X");

            //Act
            stack.BringIntoView(outer, charCol, charRow);
            
            //Assert
            stack.Render();
            console.GetDisplayReport().Verify();
        }

        [Fact]
        public void ContainingViewportsAreAllAdjustedForCharacter()
        {
            //Arrange
            var output = new Output();

            var console = new FakeFullScreenConsole(50, 20);
            var (stack, viewports) = MakeFullStack(console);
            var inner = viewports[0];
            var innerPlate = inner.Plates[0];
            var middle = viewports[1];
            var middlePlate = middle.Plates[0];

            void Run(int col, int row, string character, string comment, Plate plate)
            {
                output.WrapLine(comment);
                output.WrapLine($"(Location marked with {character})");
                output.WriteLine();
                
                plate.WriteText(col, row, character);
                var viewPort = ReferenceEquals(plate, innerPlate) ? inner : middle;
                stack.BringIntoView(viewPort, col, row);
                stack.Render();
                output.WriteLine(console.GetDisplayReport());
                output.WriteLine();
            }


            //Act
            var bottomRow = innerPlate.Height - 1;
            var rightColumn = innerPlate.Width - 1;
            Run(rightColumn, bottomRow, "A", "Bring inner bottom right into view", innerPlate);
            Run(0, 0, "B", "Bring inner top left into view", innerPlate);
            Run(0, bottomRow, "C", "Bring inner bottom left into view", innerPlate);
            Run(rightColumn, 0, "D", "Bring inner bottom left into view", innerPlate);
            Run(rightColumn - inner.WidthWithinParent + 1, inner.HeightWithinParent - 1, "E", "Already visible bottom left into view (no move required)", innerPlate);
            Run(0, 0, "F", "Bring middle plate 0,0 into view", middlePlate);
            
            //Assert
            output.Report.Verify();
        }

        [Fact]
        public void ContainingViewportsAreAllAdjustedForRectangle()
        {
            //Arrange
            var output = new Output();

            var console = new FakeFullScreenConsole(50, 20);
            var (stack, viewports) = MakeFullStack(console);
            var inner = viewports[0];
            var innerPlate = inner.Plates[0];
            var middle = viewports[1];
            var middlePlate = middle.Plates[0];

            void Run(Rectangle rectangle, char character, string comment, Plate plate)
            {
                output.WrapLine(comment);
                output.WrapLine($"(Location marked with {character})");
                output.WriteLine();

                var line = new string(character, rectangle.Width);
                for (int i = rectangle.Top; i < rectangle.Top + rectangle.Height; i++)
                {
                    plate.WriteText(rectangle.Left, i, line);
                }
                plate.WriteText(rectangle.KeyCol, rectangle.KeyRow, "@");
                
                var viewPort = ReferenceEquals(plate, innerPlate) ? inner : middle;
                stack.BringIntoView(viewPort, rectangle);
                stack.Render();
                output.WriteLine(console.GetDisplayReport());
                output.WriteLine();
            }


            //Act
            var rectangleSize = ((int width, int height))(7, 7);
            Rectangle RectangleAt(int col, int row)
            {
                return new Rectangle((col, row), rectangleSize,
                    (col + rectangleSize.width / 2, row + rectangleSize.height / 2));
            }

            var bottomRow = innerPlate.Height - 1 - rectangleSize.width;
            var rightColumn = innerPlate.Width - 1 - rectangleSize.height;
            Run(RectangleAt(rightColumn, bottomRow), 'A', "Bring inner bottom right into view", innerPlate);
            Run(RectangleAt(0, 0), 'B', "Bring inner top left into view", innerPlate);
            Run(RectangleAt(rightColumn, 0), 'C', "Bring inner top right into view", innerPlate);
            Run(RectangleAt(0, bottomRow), 'D', "Bring inner bottom left into view", innerPlate);
            Run(RectangleAt(rectangleSize.width, bottomRow - 2), 'E', "Bring inner already visible into view (no move is required)", innerPlate);

            //Assert
            output.Report.Verify();
        }

        [Fact]
        public void ViewportsPartiallyOffScreenAreRendered()
        {
            //Arrange
            var console = new FakeFullScreenConsole(50, 20);
            var inner = MakeViewport(console, "inner");
            var outer = MakeViewport(console, "outer");
            
            var innerPlate = inner.Plates[0];
            var fillString = string.Concat(Enumerable.Range(0, (innerPlate.Width / 10) + 1).Select(n => "0123456789"));
            for (int i = 0; i < innerPlate.Height; i++)
            {
                innerPlate.WriteText(0, i, fillString.Substring(i % 10, innerPlate.Width));
            }

            inner.Container = outer;
            var viewportStack = new ViewportStack(console, MakePlateStack("stack"));
            outer.Container = viewportStack.RootViewport;
            viewportStack.AddViewports(inner, outer);
            Outline(viewportStack.RootViewport, inner, outer);

            var output = new Output();

            void SetOriginAndRecord(int column, int row)
            {
                outer.SetOrigin(column, row);
                output.WrapLine($"SetOrigin({column}, {row})");
                viewportStack.Render();
                output.WriteLine(console.GetDisplayReport());
                output.WriteLine();
            }

            //Act
            SetOriginAndRecord(0, 0);
            SetOriginAndRecord(10, 0);
            SetOriginAndRecord(0, 10);
            SetOriginAndRecord(10, 10);

            //Assert
            output.Report.Verify();
        }

        private (ViewportStack stack, Viewport[] viewports) MakeFullStack(FakeFullScreenConsole console, StringBuilder sb = null)
        {
            var inner = MakeViewport(console, "inner");
            var middle = MakeViewport(console, "middle");
            var outer = MakeViewport(console, "outer");

            inner.Container = middle;
            middle.Container = outer;

            var viewportStack = new ViewportStack(console, MakePlateStack("stack"));
            outer.Container = viewportStack.RootViewport;
            viewportStack.AddViewports(inner, middle, outer);
            Outline(viewportStack.RootViewport, inner, middle, outer);

            if (sb != null)
            {
                var fs = new FakeFullScreenConsole(inner.Plates.PlateWidth, inner.Plates.PlateHeight);
                inner.Plates.Render(fs);
                sb.Append(fs.GetDisplayReport());

                middle.Plates.Render(fs);
                sb.Append(fs.GetDisplayReport());

                outer.Plates.Render(fs);
                sb.Append(fs.GetDisplayReport());
            }

            return (viewportStack, new []{ inner, middle, outer });
        }

        private void Outline(params Viewport[] all)
        {
            var chars = "+=#";
            var index = 0;
            foreach (var viewport in all)
            {
                var contains = all.FirstOrDefault(v => ReferenceEquals(v.Container, viewport));
                if (contains == null) continue;

                var plate = viewport.Plates[0];
                RenderRectangle(contains, chars[index++ % chars.Length], plate);
            }
        }
        
        private static void RenderRectangle(Viewport containedViewport, char renderChar, Plate plate)
        {
            var borderEdge = new string(renderChar, containedViewport.WidthWithinParent + 2);
            plate.WriteText(containedViewport.ColWithinParent - 1, containedViewport.RowWithinParent - 1, borderEdge);
            plate.WriteText(containedViewport.ColWithinParent - 1, containedViewport.RowWithinParent + containedViewport.HeightWithinParent, borderEdge);
            for (var i = 0; i < containedViewport.HeightWithinParent; i++)
            {
                plate.WriteText(containedViewport.ColWithinParent - 1, containedViewport.RowWithinParent + i,
                    $"{renderChar}{new string(' ', containedViewport.WidthWithinParent)}{renderChar}");
            }
        }

        private static Viewport MakeViewport(FakeFullScreenConsole console, string title)
        {
            var stack = MakePlateStack(title);
            return new Viewport(console, stack, 5, 5, 30, 10);
        }

        private static PlateStack MakePlateStack(string title)
        {
            var plate = new Plate(50, 50);
            plate.WriteText((50 - title.Length)/2, 25, title);
            var titleNumberString = title + 
                                    $" {string.Concat(Enumerable.Range(0, 50/10).Select(t => "1234567890"))}"
                                        .Substring(0, 50 - title.Length);
            plate.WriteText(0, 0, titleNumberString);
            plate.WriteText(50 - title.Length, 49, title);
            var stack = new PlateStack(plate);
            return stack;
        }
    }
}