using System;
using System.Collections.Generic;
using System.Linq;

namespace VT100.FullScreen
{
    internal static class CaptionSizer
    {
        internal static int GetLongestCaptionLength(List<ControlContainer> controls, int maxAllowableCaption)
        {
            var captionedContainers = controls.Where(c => !c.LayoutProperties.HasBorder());
            return Math.Min(captionedContainers.Max(c => (int?)c.CaptionText.Length) ?? 0, maxAllowableCaption);
        }
    }
}