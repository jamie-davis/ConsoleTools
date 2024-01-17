using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.Controls;
using VT100.FullScreen.ScreenLayers;

namespace VT100.FullScreen
{
    internal static class Positioner
    {
        public static ControlSet Position(int regionColumn, int regionRow, int regionWidth, int regionHeight, CaptionPosition captionPosition,
            IEnumerable<LayedOutControl> inputControls, LayoutProperties layoutProperties, Viewport parentViewport)
        {
            var allControls = inputControls.ToList();
            var buttons = allControls.Where(c => c.Control is ButtonControl).ToList();
            var nonButtons = allControls.Except(buttons).ToList();

            if (buttons.Any())
            {
                var buttonContainers = buttons
                    .Select(c => new ControlContainer(c.Control, c.PropertySettings, layoutProperties.ContainingViewport))
                    .ToList(); 
                var buttonBox = new ButtonBoxControl(buttonContainers);
                nonButtons.Add(new LayedOutControl(buttonBox, new List<PropertySetting>()));
            }
            
            var outcome = VerticalStackLayout.StackControls(regionColumn, regionRow, regionWidth, regionHeight, nonButtons, layoutProperties, parentViewport);
            return new ControlSet(outcome.Controls, outcome.BoxRegions, outcome.Viewports, outcome.TotalWidth, outcome.TotalHeight);
        }
    }
}