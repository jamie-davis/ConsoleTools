using System.Collections.Generic;
using System.Linq;

namespace VT100.FullScreen.ScreenLayers
{
    internal static class BoxMapMaker
    {
        internal static BoxMap Map(IEnumerable<BoxRegion> regions, int plateWidth, int plateHeight)
        {
            bool HorzRangeVisible(int x, int y, int width)
            {
                return (y >= 0 && y < plateHeight)
                       && (
                           (x >= 0 && x < plateWidth) //left is in plate
                           || (x + width - 1 >= 0 && x + width - 1 < plateWidth) //right is in plate
                           || (0 >= x && 0 < x + width) //plate left edge is in region
                           || (plateWidth - 1 >= x && plateWidth - 1 < x + width) //plate right edge is in region
                       );
            }
            
            bool VertRangeVisible(int x, int y, int width)
            {
                return (x >= 0 && x < plateWidth)
                       && (
                           (y >= 0 && y < plateHeight) //top is in plate
                           || (y + width - 1 >= 0 && y + width - 1 < plateHeight) //bottom is in plate
                           || (0 >= y && 0 < y + width) //plate top edge is in region
                           || (plateHeight - 1 >= y && plateHeight - 1 < y + width) //plote bottom edge is in region
                       );
            }
            bool Visible(BoxRegion region)
            {
                return HorzRangeVisible(region.X, region.Y, region.Width)
                    || HorzRangeVisible(region.X, region.Y + region.Height - 1, region.Width)
                    || VertRangeVisible(region.X, region.Y, region.Height)
                    || VertRangeVisible(region.X + region.Width - 1, region.Y, region.Height);
            }
            
            var boxCharacters = new BoxCharRequest[plateHeight * plateWidth];
            foreach (var boxRegion in regions.Where(Visible))
            {
                var x = boxRegion.X;
                var y = boxRegion.Y;

                var topLeft = BoxCharacterSelector.SelectTopLeft(boxRegion);
                var bottomLeft = BoxCharacterSelector.SelectBottomLeft(boxRegion);
                var topIx = CharacterArrayIndexCalculator.GetIndex(x, y, plateWidth);
                if (topIx >= 0 && x >= 0 && x < plateWidth &&  topIx < boxCharacters.Length)
                    Merge(boxCharacters, topIx, topLeft);
                var bottomIx = CharacterArrayIndexCalculator.GetIndex(x, y + boxRegion.Height - 1, plateWidth);
                if (bottomIx > 0 && bottomIx < boxCharacters.Length)
                    Merge(boxCharacters, bottomIx, bottomLeft);
                var xpos = boxRegion.X + 1;
                var horz = BoxCharacterSelector.SelectHorizontal(boxRegion);
                ++bottomIx;
                ++topIx;
                for (var n = 0; n < boxRegion.Width-2 && xpos < plateWidth; n++)
                {
                    if (topIx > 0 && xpos >= 0)
                        Merge(boxCharacters, topIx, horz);
                    if (bottomIx > 0 && bottomIx < boxCharacters.Length)
                        Merge(boxCharacters, bottomIx, horz);
                    ++xpos;
                    ++bottomIx;
                    ++topIx;
                }

                if (xpos < plateWidth)
                {
                    var topRight = BoxCharacterSelector.SelectTopRight(boxRegion);
                    var bottomRight = BoxCharacterSelector.SelectBottomRight(boxRegion);
                    if (topIx >= 0)
                        Merge(boxCharacters, topIx, topRight);
                    if (bottomIx >= 0 && bottomIx < boxCharacters.Length)
                        Merge(boxCharacters, bottomIx, bottomRight);
                    ++topIx; ++bottomIx;
                }

                var vert = BoxCharacterSelector.SelectVertical(boxRegion);
                for (int row = 1; row <= boxRegion.Height - 2; row++)
                {
                    var leftIx = CharacterArrayIndexCalculator.GetIndex(boxRegion.X, boxRegion.Y + row, plateWidth);
                    if (leftIx > 0 && leftIx < boxCharacters.Length && boxRegion.X >= 0 && boxRegion.X < plateWidth)
                        Merge(boxCharacters, leftIx, vert);
                    var rightIx = leftIx + boxRegion.Width - 1;
                    if (rightIx > 0 && rightIx < boxCharacters.Length && boxRegion.Width + boxRegion.X >= 0 && boxRegion.Width + boxRegion.X - 1 < plateWidth)
                        Merge(boxCharacters, rightIx, vert);
                }
            }

            return new BoxMap(boxCharacters, plateWidth);
        }

        private static void Merge(BoxCharRequest[] boxCharacters, int index, BoxCharRequest request)
        {
            if (boxCharacters[index].Class == null)
            {
                boxCharacters[index] = request;
                return;
            }

            if (request.RequestedLeft != null)
                boxCharacters[index].RequestedLeft = request.RequestedLeft;

            if (request.RequestedRight != null)
                boxCharacters[index].RequestedRight = request.RequestedRight;

            if (request.RequestedUp != null)
                boxCharacters[index].RequestedUp = request.RequestedUp;

            if (request.RequestedDown != null)
                boxCharacters[index].RequestedDown = request.RequestedDown;

            boxCharacters[index].Class = BoxCharacterSelector.Select(boxCharacters[index]);
        }
    }
}