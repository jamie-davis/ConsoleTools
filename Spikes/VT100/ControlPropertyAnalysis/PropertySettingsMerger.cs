using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VT100.ControlPropertyAnalysis
{
    internal static class PropertySettingsMerger
    {
        public static ReadOnlyCollection<PropertySetting> Merge(List<PropertySetting> currentSettings, ReadOnlyCollection<PropertySetting> mergeSettings)
        {
            var notReplaced = currentSettings.Where(c => mergeSettings.All(m => m.Property != c.Property));
            var merged = notReplaced.Concat(mergeSettings).ToList();
            return new ReadOnlyCollection<PropertySetting>(merged);
        }
    }
}