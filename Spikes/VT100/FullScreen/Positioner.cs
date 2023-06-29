using System;
using System.Collections.Generic;
using System.Linq;
using VT100.FullScreen.Controls;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    internal static class Positioner
    {

        public static ControlSet Position(int regionColumn, int regionRow, int regionWidth, int regionHeight, CaptionAlignment captionAlignment,
            IEnumerable<LayedOutControl> inputControls, LayoutProperties layoutProperties)
        {
            var (boxRegions, controls, width, height) = CalculatePositions(regionColumn, regionRow, regionWidth, regionHeight, captionAlignment, inputControls, layoutProperties);
            return new ControlSet(controls, boxRegions, width, height);
        }

        private static (List<BoxRegion> BoxRegions, List<ControlContainer> Controls, int TotalWidth, int TotalHeight) CalculatePositions(int regionColumn, int regionRow, int regionWidth, int regionHeight,
            CaptionAlignment captionAlignment, IEnumerable<LayedOutControl> inControls, LayoutProperties layoutProperties)
        {
            List<BoxRegion> _boxRegions = new();

            List<ControlContainer> orignalControls;
            List<ControlContainer> buttons;

            var hasBorder = layoutProperties.HasBorder();

            var width = hasBorder ? regionWidth - 2 : regionWidth;
            var height = hasBorder ? regionHeight - 2 : regionHeight;

            var row = hasBorder ? regionRow + 2 : regionRow + 1;
            var column = hasBorder ? regionColumn + 2 : regionColumn + 1;

            var allControls = inControls.Select(c => new ControlContainer(c.Control, c.PropertySettings)).ToList();
            orignalControls = allControls.Where(c => c.Control is not ButtonControl).ToList();
            buttons = allControls.Where(c => c.Control is ButtonControl).ToList();

            var maxAllowableCaption = regionWidth - 4;

            int GetLongestCaptionLength()
            {
                var captionedContainers = orignalControls.Where(c => !c.LayoutProperties.HasBorder());
                return Math.Min(captionedContainers.Max(c => (int?)c.CaptionText.Length) ?? 0, maxAllowableCaption);
            }

            var maxCaption = GetLongestCaptionLength();

            foreach (var container in orignalControls.ToList())
            {
                var inBorder = container.LayoutProperties.HasBorder();

                if (container.Control is IRegionControl)
                    RenderRegion();
                else
                    RenderControl();

                row += container.Height + 1;

                //Rendering for a region
                void RenderRegion()
                {
                    var controlCol = inBorder ? column + 1 : column;
                    var controlRow = inBorder ? row + 1 : row;
                    var controlWidth = width - controlCol - (inBorder ? 4 : 3);

                    var controlSet = ((IRegionControl)container.Control).ComputePosition(container, controlCol, controlRow, controlWidth, height);
                   MergeContainerControls();
                    _boxRegions.AddRange(controlSet.ExportBoxRegions()); 

                    void MergeContainerControls()
                    {
                        var controlContainers = controlSet.ExportControls().Reverse();
                        var index = allControls.IndexOf(container);
                        foreach (var nestedContainer in controlContainers) allControls.Insert(index, nestedContainer);
                    }
                }
                
                //Rendering for a standard control
                void RenderControl()
                {
                    container.LabelControl.Caption = container.CaptionText;
                    var captionLength = inBorder ? container.LabelControl.Caption.Length : maxCaption;
                    var captionCol = inBorder ? column + 1 : column;
                    var captionRow = inBorder ? row + 1 : row;
                    container.LabelControl.Position(captionCol, captionRow, captionLength, 1);

                    var controlX = captionCol + captionLength + 2;
                    var controlWidth = width - controlX - (inBorder ? 1 : 0);
                    container.Control.Position(controlX, captionRow, controlWidth, 1);
                }
            }

            var space = buttons.Select(b => b.Control.GetRequestedSize()).ToList();
            var maxHeight = space.Max(m => (int?)m.Height) ?? 0;
            var fullWidth = (space.Sum(m => (int?)m.Width) ?? 0) + space.Count - 1;
            row += maxHeight;
            var buttonColumn = (width - fullWidth) / 2;
            foreach (var container in buttons)
            {
                var requestedSize = container.Control.GetRequestedSize();
                var controlY = row - requestedSize.Height;
                var controlX = buttonColumn;
                buttonColumn += requestedSize.Width + 1;
                container.Control.Position(controlX, controlY, requestedSize.Width, requestedSize.Height);
            }

            foreach (var borderedContainer in allControls.Where(c => c.LayoutProperties.HasBorder()))
            {
                _boxRegions.Add(borderedContainer.RenderBorder());
            }

            var baseHeight = (allControls.Any() ? row : row - 1) - regionRow;
            var totalHeight = baseHeight + (buttons.Any() ? 1 : 0); //add a row below the buttons
            if (hasBorder)
            {
                _boxRegions.Add(new BoxRegion(regionColumn, regionRow, regionWidth, totalHeight, LineWeight.Light));
            }

            return (_boxRegions, allControls, regionWidth, totalHeight);
        }
    }

    internal interface IRegionControl
    {
        ControlSet ComputePosition(ControlContainer controlContainer, int regionCol, int regionRow, int width,
            int height);
    }
}