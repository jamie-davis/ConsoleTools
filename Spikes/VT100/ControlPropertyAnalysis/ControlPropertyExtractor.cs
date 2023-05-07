using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using VT100.Attributes;

namespace VT100.ControlPropertyAnalysis
{
    public static class PropertyNameConverter
    {
        public static string FromAttributeTypeName(string attribTypeName)
        {
            if (attribTypeName.EndsWith("Attribute") && attribTypeName.Length > 9)
                return attribTypeName.Substring(0, attribTypeName.Length - 9);

            return attribTypeName;
        }
    }

    /// <summary>
    /// This class analyses a type or property definition to extract any control property meta data defined on it. 
    /// </summary>
    public static class ControlPropertyExtractor
    {
        internal static ReadOnlyCollection<Type> PropertyAttributes { get; } = FindPropertyAttributes();

        private static ReadOnlyCollection<Type> FindPropertyAttributes()
        {
            var assembly = typeof(BackgroundAttribute).Assembly;
            var attribs = assembly.GetTypes()
                .Where(t => typeof(Attribute).IsAssignableFrom(t) && t.GetCustomAttribute<PropertyAttributeAttribute>(true) != null && !t.IsAbstract)
                .ToList();
            return new ReadOnlyCollection<Type>(attribs);
        }

        public static List<PropertySetting> Extract(Type type)
        {
            return Extract(type?.GetCustomAttributes(true).Cast<Attribute>());
        }

        public static List<PropertySetting> Extract(PropertyInfo? propertyInfo)
        {
            var customAttributes = propertyInfo?.GetCustomAttributes(true);
            return Extract(customAttributes?.Cast<Attribute>());
        }

        public static List<PropertySetting> Extract(MethodInfo? methodInfo)
        {
            return Extract(methodInfo?.GetCustomAttributes(true).Cast<Attribute>());
        }

        private static List<PropertySetting> Extract(IEnumerable<Attribute> attributes)
        {
            if (attributes == null) return new List<PropertySetting>();
            
            return attributes
                .Where(a => PropertyAttributes.Contains(a.GetType()))
                .Select(MakePropertySetting)
                .ToList();
        }

        private static PropertySetting MakePropertySetting(Attribute attribInstance)
        {
            var attribType = attribInstance.GetType();
            var propertyName = PropertyNameConverter.FromAttributeTypeName(attribType.Name);
            
            var valueProp = attribType.GetProperties().FirstOrDefault();
            if (valueProp == null) return new PropertySetting(propertyName);

            var parameterisedType = typeof(PropertySetting<>);
            var specificType = parameterisedType.MakeGenericType(valueProp.PropertyType);
            var instance = (PropertySetting)Activator.CreateInstance(specificType, propertyName, valueProp.GetValue(attribInstance));
            return instance;
        }
    }
}