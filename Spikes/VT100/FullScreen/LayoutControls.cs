using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Vt100.Attributes;
using Vt100.FullScreen.Controls;

namespace Vt100.FullScreen
{
    internal static class LayoutControls
    {
        private static Dictionary<Type, Type> _controlLookup;

        public static IEnumerable<ILayoutControl> Extract(IFullScreenApplication app, ILayout layout)
        {
            if (layout == null)
                yield break;

            var props = layout.GetType().GetProperties()
                .Select(p => new {Property = p, Control = GetControl(app, layout, p)})
                .Where(p => p.Control != null)
                .ToList();

            foreach (var prop in props)
            {
                yield return prop.Control;
            }
        }

        private static ILayoutControl GetControl(IFullScreenApplication app, ILayout layout, PropertyInfo propertyInfo)
        {
            if (_controlLookup == null) LoadControls();

            var attribs = propertyInfo.GetCustomAttributes().Where(a => _controlLookup.ContainsKey(a.GetType())).ToList();
            if (attribs.Count > 1)
                throw new Exception($"{propertyInfo.Name} : Element must have a single control type.");

            if (attribs.Count == 1)
            {
                var controlAttrib = attribs[0];
                var attribType = controlAttrib.GetType();
                var controlType = _controlLookup[attribType];
                var control = Activator.CreateInstance(controlType) as ILayoutControl;
                if (control != null)
                {
                    control.Bind(app, layout, propertyInfo.GetValue, MakeUpdater(propertyInfo));
                    var acceptMethod = control.GetType().GetMethod("AcceptConfig", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (acceptMethod != null && acceptMethod.GetParameters().Length == 1 && acceptMethod.GetParameters()[0].ParameterType == attribType)
                    {
                        acceptMethod.Invoke(control, new object[] {controlAttrib});
                    }
                    return control;
                }
            }
            return null;
        }

        private static Action<object, object> MakeUpdater(PropertyInfo propertyInfo)
        {
            var genericMethod = typeof(LayoutControls).GetMethod(nameof(PropertyUpdate), BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(genericMethod != null, nameof(genericMethod) + " != null");
            var method = genericMethod.MakeGenericMethod(propertyInfo.PropertyType);
            return (o, v) => method.Invoke(null, new [] {propertyInfo, o, v});
        }

        private static void PropertyUpdate<T>(PropertyInfo prop, object instance, object value)
        {
            if (value == null)
                value = default(T);

            var valueType = value?.GetType();
            if (valueType != null)
            {
                if (valueType != typeof(T)) 
                    value = Convert.ChangeType(value, typeof(T));
            }

            prop.SetValue(instance, value);
        }


        private static void LoadControls()
        {
            var sampleControlType = typeof(TextBox);
            var controlTypes = sampleControlType.Assembly.GetTypes()
                .Where(t => t.Namespace == sampleControlType.Namespace)
                .Select(t => new {Type = t, ControlAttribute = t.GetCustomAttribute<ControlAttribute>()})
                .Where(t => t.ControlAttribute != null);
            _controlLookup = controlTypes.ToDictionary(t => t.ControlAttribute.IntroducingAttribute, t => t.Type);
        }
    }
}