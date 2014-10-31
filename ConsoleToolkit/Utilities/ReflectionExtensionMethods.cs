using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleToolkit.Utilities
{
    /// <summary>
    /// This class defines some extension methods to mimic the .NET 4.5 reflection method changes.
    /// </summary>
    internal static class ReflectionExtensionMethods
    {
        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            var attribute = type.GetCustomAttributes(typeof (T), true).FirstOrDefault();
            if (attribute != null)
                return attribute as T;

            return null;
        }

        public static T GetCustomAttribute<T>(this MemberInfo prop) where T : Attribute
        {
            var attribute = prop.GetCustomAttributes(typeof (T), true).FirstOrDefault();
            if (attribute != null)
                return attribute as T;

            return null;
        }

        public static T GetCustomAttribute<T>(this MethodInfo method) where T : Attribute
        {
            var attribute = method.GetCustomAttributes(typeof (T), true).FirstOrDefault();
            if (attribute != null)
                return attribute as T;

            return null;
        }

        public static object GetValue(this PropertyInfo prop, object instance)
        {
            return prop.GetValue(instance, null);
        }
    }
}
