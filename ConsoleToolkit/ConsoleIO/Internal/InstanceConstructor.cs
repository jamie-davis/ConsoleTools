using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class InstanceConstructor<T>
    {
        internal class NoConstructorWithParametersMathingPropertyFound : Exception
        {
        }
        public static T MakeInstance<T2>(IEnumerable<T2> properties) where T2 : IPropertySource
        {
            var props = properties.Cast<IPropertySource>().ToList();
            if (HasConstructor(props))
                return MakeInstanceUsingConstructor(props);

            return MakeInstanceUsingMemberSetters(props);
        }

        private static bool HasConstructor<T2>(IEnumerable<T2> properties) where T2 : IPropertySource
        {
            var constructors = typeof(T).GetConstructors()
                .Where(c => ConstructorHasMatchingParameters(c, properties));
            var num = constructors.Count();
            return num == 1;
        }

        public static T MakeInstanceUsingConstructor<T2>(IEnumerable<T2> properties) where T2 : IPropertySource
        {
            var ctor = typeof (T).GetConstructors().FirstOrDefault(c => ConstructorHasMatchingParameters(c, properties.Cast<IPropertySource>()));
            if (ctor == null)
                throw new NoConstructorWithParametersMathingPropertyFound();
 
            var args = properties.Select(item => item.Value).ToArray();
            return (T) ctor.Invoke(args);
        }

        public static T MakeInstanceUsingMemberSetters(IEnumerable<IPropertySource> properties)
        {
            var instance = Activator.CreateInstance(typeof (T));
            foreach (var item in properties)
            {
                item.Property.SetValue(instance, item.Value, null);
            }
            return (T) instance;
        }

        private static bool ConstructorHasMatchingParameters<T2>(ConstructorInfo ctor, IEnumerable<T2> properties) where T2 : IPropertySource
        {
            var props = properties
                .Select((p, i) => new { Index = i, Type = p.Property.PropertyType })
                .ToList();
            var ctorParams = ctor.GetParameters()
                .Select((p, i) => new { Index = i, Type = p.ParameterType });

            return props.Intersect(ctorParams).Count() == props.Count();
        }

    }
}