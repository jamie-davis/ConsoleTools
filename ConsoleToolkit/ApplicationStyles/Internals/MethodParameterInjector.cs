using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.Exceptions;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal class MethodParameterInjector
    {
        private List<object> _injectableReferences;
        private Dictionary<Type, object> _instancesForSpecifiedTypes;

        public MethodParameterInjector(IEnumerable<object> injectableReferences, IEnumerable<KeyValuePair<Type, object>> instancesForSpecifiedTypes = null)
        {
            if (injectableReferences == null)
                _injectableReferences = new List<object>();
            else
                _injectableReferences = injectableReferences.ToList();

            if (instancesForSpecifiedTypes != null)
                _instancesForSpecifiedTypes = instancesForSpecifiedTypes.ToDictionary(i => i.Key, i => i.Value);
            else
                _instancesForSpecifiedTypes = new Dictionary<Type, object>();
        }

        public object[] GetParameters(MethodInfo method, IEnumerable<object> oneTimeInjectables)
        {
            var localInjectables = oneTimeInjectables.ToList();
            var injected = new List<object>();
            var parameters = method.GetParameters();
            foreach (var parameterInfo in parameters)
            {
                var parameterType = parameterInfo.ParameterType;
                object value;
                if (!_instancesForSpecifiedTypes.TryGetValue(parameterType, out value))
                {
                    value = localInjectables.FirstOrDefault(i => Matches(parameterType, i.GetType()))
                                ?? _injectableReferences.FirstOrDefault(i => Matches(parameterType, i.GetType()));
                }

                if (value == null)
                {
                    throw new ParameterTypeCannotBeResolved(parameterType);
                }

                injected.Add(value);
            }
            return injected.ToArray();
        }

        private bool Matches(Type requestedType, Type cachedType)
        {
            return requestedType.IsAssignableFrom(cachedType);
        }

        public bool CanSupply(Type parameterType)
        {
            return _instancesForSpecifiedTypes.ContainsKey(parameterType) ||
                _injectableReferences.Any(i => Matches(parameterType, i.GetType()));
        }

        public void AddInstance<T>(T instance)
        {
            _instancesForSpecifiedTypes[typeof (T)] = instance;
        }

        public T GetParameter<T>()
        {
            object value;
            if (!_instancesForSpecifiedTypes.TryGetValue(typeof(T), out value))
            {
                var matchedType = _injectableReferences.FirstOrDefault(i => Matches(typeof (T), i.GetType()));
                return matchedType == null ? default(T) : (T)matchedType;
            }

            return (T)value;
        }
    }
}