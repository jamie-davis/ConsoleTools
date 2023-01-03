using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using VT100.FullScreen;

namespace VT100.ControlPropertyAnalysis
{
    internal static class ControlSettingsUpdater
    {
        public static void Update(ILayoutControl control, List<PropertySetting> parentSettings, ReadOnlyCollection<PropertySetting> controlSettings)
        {
            if (control == null) return;

            var type = control.GetType();
            var controlFormattedType = type.GetInterfaces().FirstOrDefault(i =>
                i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IFormattedLayoutControl<>));
            if (controlFormattedType == null) return;

            var untypedMethod = typeof(ControlSettingsUpdater).GetMethod(nameof(UpdateSettings), BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(untypedMethod != null, "Unable to find handler method");
            var genericMethod = untypedMethod.MakeGenericMethod(controlFormattedType.GetGenericArguments()[0]);
            Debug.Assert(genericMethod != null, "Unable to make generic handler method");
            genericMethod.Invoke(null, new object[] { control, PropertySettingsMerger.Merge(parentSettings, controlSettings) });
        }

        private static void UpdateSettings<T>(IFormattedLayoutControl<T> control, ReadOnlyCollection<PropertySetting> settings)
        where T : class, new()
        {
            if (control.Format == null)
                control.Format = new T();
            
            ControlPropertySetter.Set(control.Format, settings);
        }
    }
}