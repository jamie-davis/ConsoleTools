using System;
using System.Text;
using VT100.FullScreen.ControlBehaviour;

namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// A viewport on a virtualised console. The viewport allows a larger surface to be displayed than the physical
    /// display will allow. The viewport can be repositioned over the contained surface to bring different parts
    /// of the contained data into view.
    /// </summary>
    internal class Viewport
    {
        private readonly IFullScreenConsole _console;

        public Viewport(IFullScreenConsole console, PlateStack plateStack, int col, int row, int width, int height)
        {
            _console = console;
            Plates = plateStack;
            ColWithinParent = col;
            RowWithinParent = row;
            WidthWithinParent = width;
            HeightWithinParent = height;
            FirstContainedVisibleColumn = 0;
            FirstContainedVisibleRow = 0;
        }

        /// <summary>
        /// Column where in the parent console our Viewport is showing
        /// </summary>
        public int ColWithinParent { get; private set; }
        
        /// <summary>
        /// Row where in the parent console our Viewport is showing
        /// </summary>
        public int RowWithinParent { get; private set; }
        
        /// <summary>
        /// The width of the viewport within the parent console
        /// </summary>
        public int WidthWithinParent { get; private set; }

        /// <summary>
        /// Height of the viewport within the parent console
        /// </summary>
        public int HeightWithinParent { get; private set; }

        /// <summary>
        /// The width of the console being viewed through the viewport
        /// </summary>
        public int ContainedConsoleWidth => Plates.PlateWidth;

        /// <summary>
        /// The height of the console being viewed through the viewport
        /// </summary>
        public int ContainedConsoleHeight => Plates.PlateHeight;
        
        /// <summary>
        /// The column on which the viewport is positioned - this is the column of the character
        /// in the contained console that is visible in the top left of the viewport.
        /// </summary>
        public int FirstContainedVisibleColumn { get; private set; }
        
        /// <summary>
        /// The row on which the viewport is positioned - this is the row of the character
        /// in the contained console that is visible in the top left of the viewport.
        /// </summary>
        public int FirstContainedVisibleRow { get; private set; }

        /// <summary>
        /// The viewport that contains this viewport. This will be none if it is not contained.
        /// </summary>
        public Viewport Container { get; set; }

        /// <summary>
        /// The <see cref="PlateStack"/> providing content for this viewport.
        /// </summary>
        public PlateStack Plates { get; }

        /// <summary>
        /// Render to the console. This viewport may well be inside another viewport. If that is the case, that viewport
        /// will be showing this viewport at an offset in the console. This is described using <see cref="parentConsoleRootCol"/>
        /// and <see cref="parentConsoleRootRow"/> which indicate where our parent viewport is currently showing on the console.
        /// Our coordinates must be adjusted relative to this. One of the results of this adjustment is that we may end up
        /// partially or completely outside the limits of the console area in which we are allowed to draw. This is itself 
        /// </summary>
        /// <param name="parentConsoleRootCol">If we are inside another viewport, this will be the offset of that viewport
        /// in the console. We need to adjust our coordinates to take that into account.</param>
        /// <param name="parentConsoleRootRow">If we are inside another viewport, this will be the offset of that viewport
        /// in the console. We need to adjust our coordinates to take that into account.</param>
        /// <param name="absoluteColLimit">If we are inside another viewport, this will be the limit of that viewport's
        /// visual area and we need to make sure we don't draw outside of this limit.</param>
        /// <param name="absoluteRowLimit">If we are inside another viewport, this will be the limit of that viewport's
        /// visual area and we need to make sure we don't draw outside of this limit.</param>
        /// <param name="visibleColOffset">If we are inside another viewport, we need to be able to determine which
        /// fragment of our display is within the visible limits of the parent. This parameter is an adjustment to the
        /// parent viewport's content which we need to use to adjust our effective coordinates within the parent.</param>
        /// <param name="visibleRowOffset">If we are inside another viewport, we need to be able to determine which
        /// fragment of our display is within the visible limits of the parent. This parameter is an adjustment to the
        /// parent viewport's content which we need to use to adjust our effective coordinates within the parent.</param>
        public void Render(int parentConsoleRootCol, int parentConsoleRootRow, Rectangle visibleRegion)
        {
            visibleRegion ??= new Rectangle((0, 0), (_console.WindowWidth, _console.WindowHeight), (-1, -1));
            
            var ourCol = parentConsoleRootCol + ColWithinParent;
            var ourRow = parentConsoleRootRow + RowWithinParent;
            
            using (new CursorHider())
            {
                for (var line = Math.Max(0, visibleRegion.Top - ourRow); 
                     line < HeightWithinParent && line + ourRow < visibleRegion.Top + visibleRegion.Height; 
                     line++)
                {
                    _console.SetCursorPosition(Math.Max(ourCol, visibleRegion.Left), ourRow + line);
                    var sb = new StringBuilder();
                    for (var index = Math.Max(0, visibleRegion.Left - ourCol); 
                         index < WidthWithinParent && index + ourCol < visibleRegion.Left + visibleRegion.Width; 
                         index++)
                    {
                        var next = Plates.TakeCharacterFromStack(index + FirstContainedVisibleColumn, line + FirstContainedVisibleRow);
                        sb.Append(next);
                    }
                    _console.Write(sb.ToString());
                }
            }
        }

        public void SetOrigin(int column, int row)
        {
            FirstContainedVisibleColumn = column;
            FirstContainedVisibleRow = row;
        }

        public (int col, int row) BringIntoView(int column, int row)
        {
            int SelectValue(int min, int max, int current)
            {
                if (current >= min && current <= max) return current;
                return current < min ? min : max;
            }

            var minLeft = column - WidthWithinParent + 1; //e.g. 6 - 3 + 1 = 4, 4, 5 and 6 are in the visible range 
            var maxLeft = column;

            var minTop = row - HeightWithinParent + 1; //e.g. 6 - 3 + 1 = 4, 4, 5 and 6 are in the visible range
            var maxTop = row;

            var requiredOriginCol = SelectValue(minLeft, maxLeft, FirstContainedVisibleColumn);
            var requiredOriginRow = SelectValue(minTop, maxTop, FirstContainedVisibleRow);
            
            if (FirstContainedVisibleColumn != requiredOriginCol || FirstContainedVisibleRow != requiredOriginRow)
                SetOrigin(requiredOriginCol, requiredOriginRow);

            return (ColWithinParent + (column - requiredOriginCol), RowWithinParent + (row - requiredOriginRow));
        }

        public Rectangle BringIntoView(Rectangle requestedRectangle)
        {
            var rectangle = RectangleReducer.Reduce(requestedRectangle, WidthWithinParent, HeightWithinParent);

            Rectangle ComputeActual()
            {
                (int left, int top) topLeftPoint = (rectangle.Left - FirstContainedVisibleColumn + ColWithinParent, rectangle.Top - FirstContainedVisibleRow + RowWithinParent);
                var keyPoint = (rectangle.KeyCol - rectangle.Left + topLeftPoint.left, rectangle.KeyRow - rectangle.Top + topLeftPoint.top);
                return new Rectangle(topLeftPoint, (rectangle.Width, rectangle.Height), keyPoint);
            }

            bool ColVisible(int col)
            {
                return col >= FirstContainedVisibleColumn && col < FirstContainedVisibleColumn + WidthWithinParent;
            }

            bool RowVisible(int row)
            {
                return row >= FirstContainedVisibleRow && row < FirstContainedVisibleRow + HeightWithinParent;
            }
            
            //first deal with cases where one edge of the rectangle is in range
            var right = rectangle.Left + rectangle.Width - 1;
            var bottom = rectangle.Top + rectangle.Height - 1;
            var bringIntoViewCol = -1;
            var bringIntoViewRow = -1;
            if (ColVisible(rectangle.Left) && ColVisible(right)) bringIntoViewCol = rectangle.Left;
            if (RowVisible(rectangle.Top) && RowVisible(bottom)) bringIntoViewRow = rectangle.Top;

            if (bringIntoViewCol < 0)
                bringIntoViewCol = rectangle.Left < FirstContainedVisibleColumn ? rectangle.Left : right;

            if (bringIntoViewRow < 0)
                bringIntoViewRow = rectangle.Top < FirstContainedVisibleRow ? rectangle.Top : bottom;
            
            BringIntoView(bringIntoViewCol, bringIntoViewRow);
            return ComputeActual();
        }
    }
}