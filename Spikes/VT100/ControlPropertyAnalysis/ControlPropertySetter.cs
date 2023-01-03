using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VT100.ControlPropertyAnalysis
{
    internal static class ControlPropertySetter
    {
        public static void Set<T>(T propClass, IReadOnlyCollection<PropertySetting> props) where T : class
        {
            if (propClass == null) return;

            foreach (var property in typeof(T).GetProperties())
            {
                if (TryFindMatch(property, props, out object value))
                {
                    property.SetValue(propClass, value);
                }
            }
        }

        private static bool TryFindMatch(PropertyInfo property, IReadOnlyCollection<PropertySetting> props, out object value)
        {
            var nameMatch = props.FirstOrDefault(p => p.Property == property.Name && p.GetValueType() != null && property.PropertyType.IsAssignableFrom(p.GetValueType()));
            if (nameMatch != null)
            {
                value = nameMatch.GetValue();
                return true;
            }

            value = null;
            return false;
        }
    }
}