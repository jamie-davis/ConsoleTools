using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using VT100.FullScreen.Controls;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    /// <summary>
    /// Class responsible for the layout of controls in a vertical stack. This class can return both the  
    /// </summary>
    internal static class VerticalStackLayout
    {
        /// <summary>
        /// Layout a set of controls in a vertical stack. Labels will be generated with an equal width and to the left
        /// of each control.
        /// </summary>
        /// <param name="regionColumn">The start column of the area available for use</param>
        /// <param name="regionRow">The start column of the area available for use</param>
        /// <param name="regionWidth">The width of the area available for use</param>
        /// <param name="regionHeight">The height of the area available for use</param>
        /// <param name="inControls">The console to lay out</param>
        /// <param name="layoutProperties">The properties that control the layout</param>
        /// <param name="containingViewport">The viewport in which the rendering will take place</param>
        /// <returns>An object containing the laid out results</returns>
        internal static PositioningOutcome StackControls(int regionColumn, int regionRow, int regionWidth,
            int regionHeight, IEnumerable<LayedOutControl> inControls,
            LayoutProperties layoutProperties, Viewport containingViewport)
        {
            List<BoxRegion> boxRegions = new();
            List<Viewport> viewports = new();

            List<ControlContainer> originalControls;
            List<ControlContainer> buttons;

            var (hasBorder, usableRect) = RegionRectCalculator.Calculate(regionColumn, regionRow, regionWidth, regionHeight, layoutProperties);
            
            var column = usableRect.Left;
            var row = usableRect.Top;
            var width = usableRect.Width;
            var height = usableRect.Height;
            
            var allControls = inControls.Select(c => new ControlContainer(c.Control, c.PropertySettings, layoutProperties.ContainingViewport)).ToList();
            originalControls = allControls.Where(c => c.Control is not ButtonControl).ToList();
            buttons = allControls.Where(c => c.Control is ButtonControl).ToList();

            var maxAllowableCaption = regionWidth - (layoutProperties.CaptionPosition == CaptionPosition.Top ? 0 : 4);
            var maxCaption = CaptionSizer.GetLongestCaptionLength(originalControls, maxAllowableCaption);

            foreach (var container in originalControls.ToList())
            {
                var inBorder = container.LayoutProperties.HasBorder();

                if (container.Control is IRegionControl)
                    PositionRegion();
                else
                    PositionControl();

                row += container.Height + 1;

                //Rendering for a region
                void PositionRegion()
                {
                    var controlCol = inBorder ? column + 1 : column;
                    var controlRow = inBorder ? row + 1 : row;
                    var controlWidth = width - controlCol - (inBorder ? 4 : 0);

                    var controlSet = ((IRegionControl)container.Control).ComputePosition(container, controlCol, controlRow, controlWidth, height, containingViewport);
                    MergeContainerControls();
                    boxRegions.AddRange(controlSet.ExportBoxRegions());
                    viewports.AddRange(controlSet.ExportViewports());
                    
                    void MergeContainerControls()
                    {
                        var controlContainers = controlSet.ExportControls().Reverse();
                        var index = allControls.IndexOf(container);
                        foreach (var nestedContainer in controlContainers) allControls.Insert(index, nestedContainer);
                    }
                }
                
                //Rendering for a standard control (note that standard controls may not contain viewports)
                void PositionControl()
                {
                    container.LabelControl.Caption = container.CaptionText;
                    var captionLength = inBorder ? container.LabelControl.Caption.Length : maxCaption;
                    var captionCol = inBorder ? column + 1 : column;
                    var captionRow = inBorder ? row + 1 : row;
                    container.LabelControl.Position(captionCol, captionRow, captionLength, 1);
                    int controlX, controlWidth, controlRow;
                    if (layoutProperties.CaptionPosition == CaptionPosition.Left)
                    {
                        controlX = captionCol + captionLength + 2;
                        controlWidth = width - controlX - (inBorder ? 1 : 0);
                        controlRow = captionRow;
                    }
                    else
                    {
                        controlX = captionCol;
                        controlWidth = width - controlX - (inBorder ? 1 : 0);
                        controlRow = captionRow + 1;
                        container.AdditionalControlHeight(1);
                    }
                    container.Control.Position(controlX, controlRow, controlWidth, 1);
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
                boxRegions.Add(borderedContainer.RenderBorder());
            }

            var baseHeight = (allControls.Any() ? row : row - 1) - regionRow;
            var totalHeight = baseHeight + (buttons.Any() ? 1 : 0); //add a row below the buttons
            if (hasBorder)
            {
                boxRegions.Add(new BoxRegion(regionColumn, regionRow, regionWidth, totalHeight, LineWeight.Light));
            }

            foreach (var viewport in viewports) viewport.Container = layoutProperties.ContainingViewport;
            return new (boxRegions, allControls, viewports, regionWidth, totalHeight);
        }

        internal static MeasurementOutcome ComputeSizeLimits(IEnumerable<LayedOutControl> inControls,
            LayoutProperties layoutProperties, int availableWidth, int availableHeight)
        {
            return new MeasurementOutcome((0,0), (availableWidth, availableHeight));
        }
    }
}