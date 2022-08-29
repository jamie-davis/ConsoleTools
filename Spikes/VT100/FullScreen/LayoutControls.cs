using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using VT100.Attributes;
using VT100.FullScreen.Controls;

namespace VT100.FullScreen
{
    internal static class LayoutControls
    {
        private static Dictionary<Type, Type> _controlLookup;
        private static object[] noParams = {};

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

            var methods = layout.GetType().GetMethods()
                .Select(m => new {Method = m, Control = GetControl(app, layout, methodInfo: m)})
                .Where(m => m.Control != null)
                .ToList();

            foreach (var method in methods)
            {
                yield return method.Control;
            }
            
                        
            if (CancelRequired(layout, out var cancelCaption))
                yield return MakeCancelButton(app, layout, cancelCaption);

        }

        private static ButtonControl MakeCancelButton(IFullScreenApplication app, ILayout layout,
            string caption)
        {
            var button = new ButtonControl();
            button.MethodBind(app, layout, o => true);
            button.AcceptConfig(new ButtonAttribute(caption ?? "Cancel", ExitMode.ExitOnSuccess));
            return button;
        }

        private static bool CancelRequired(ILayout layout, out string cancelCaption)
        {
            var attribute = layout.GetType().GetCustomAttribute<ScreenAttribute>();
            if (attribute == null)
            {
                cancelCaption = null;
                return false;
            }

            cancelCaption = attribute.ExitButtonCaption;
            return true;
        }

        private static ILayoutControl GetControl(IFullScreenApplication app, ILayout layout, PropertyInfo propertyInfo = null, MethodInfo methodInfo = null)
        {
            if (_controlLookup == null) LoadControls();

            var attribs = propertyInfo?.GetCustomAttributes().Where(a => _controlLookup.ContainsKey(a.GetType())).ToList()
                ?? methodInfo?.GetCustomAttributes().Where(a => _controlLookup.ContainsKey(a.GetType())).ToList();
            if (attribs.Count > 1)
                throw new Exception($"{propertyInfo?.Name ?? methodInfo?.Name} : Element must have a single control type.");

            if (attribs.Count == 1)
            {
                var controlAttrib = attribs[0];
                var attribType = controlAttrib.GetType();
                var controlType = _controlLookup[attribType];
                var control = Activator.CreateInstance(controlType) as ILayoutControl;
                if (control != null)
                {
                    if (propertyInfo != null)
                    {
                        Func<object,object> getter = propertyInfo.GetValue;
                        var updater = MakeUpdater(propertyInfo);
                        control.PropertyBind(app, layout, getter, updater);
                    }

                    if (methodInfo != null)
                    {
                        Func<object, bool> methodRunner = MakeMethodCall(methodInfo);
                        control.MethodBind(app, layout, methodRunner);
                    }

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

        private static Func<object, bool> MakeMethodCall(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType == typeof(bool))
            {
                return o => (bool)(methodInfo.Invoke(o, noParams) ?? true);
            }

            return o =>
            {
                methodInfo.Invoke(o, noParams);
                return true;
            };
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