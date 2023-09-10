using System.Linq;
using VT100.FullScreen.ScreenLayers;
using VT100.Tests.Fakes;

namespace VT100.Tests.TestUtilities
{
    internal class ViewportTestRig
    {
        public FakeFullScreenConsole Console { get; }
        public Viewport Vp { get; }
        public PlateStack Plates { get; }
        public Plate Plate { get; set; }

        /// <summary>
        /// For the tests, the viewport's captured console will be double the width and height of the visible
        /// part. The visible part will be positioned centrally in the root console.
        /// </summary>
        public ViewportTestRig(int consoleWidth, int consoleHeight, int viewportWidth, int viewportHeight,
            RigOption rigOption = RigOption.Default)
        {
            Console = new(consoleWidth, consoleHeight);
            var plateWidth = consoleWidth * 2;
            var plateHeight = consoleHeight * 2;
            var plate = new Plate(plateWidth, plateHeight);
            var plates = new PlateStack(plate);

            var viewportLeft = (consoleWidth - viewportWidth) / 2;
            var viewportTop = (consoleHeight - viewportHeight) / 2;

            var borderLeft = viewportLeft - 1;
            var borderWidth = viewportWidth + 2;
            
            Console.SetCursorPosition(borderLeft,viewportTop - 1);
            Console.Write(new string('*', borderWidth));
            Console.SetCursorPosition(borderLeft,viewportTop + viewportHeight);
            Console.Write(new string('*', borderWidth));
            
            for (int i = viewportTop; i < viewportTop + viewportHeight; i++)
            {
                Console.SetCursorPosition(borderLeft,i);
                Console.Write('*');
                Console.SetCursorPosition(borderLeft + viewportWidth + 1,i);
                Console.Write('*');
            }
            Vp = new Viewport(Console, plates, viewportLeft, viewportTop, viewportWidth, viewportHeight);
            Plates = plates;
            Plate = plate;
            
            if (rigOption == RigOption.Default)
            {
                var letterString = string.Concat(Enumerable.Range(0,plateWidth - 5).Select(n => (char)('A' + n % 26)));
                for (var i = 0; i < plateHeight; i++) plate.WriteText(0, i, $"{i:00}: {letterString}");
            }
        }

        public string GetDisplayReport(DisplayReportOptions options = DisplayReportOptions.Default)
        {
            return Console.GetDisplayReport(options);
        }

        public (int column, int row) GetOrigin()
        {
            return (Vp.FirstContainedVisibleColumn, Vp.FirstContainedVisibleRow);
        }

        public Rectangle GetRectangle(int keyCol, int keyRow)
        {
            return new Rectangle((Vp.ColWithinParent, Vp.RowWithinParent), (Vp.WidthWithinParent, Vp.HeightWithinParent), (keyCol, keyRow));
        }
    }
}