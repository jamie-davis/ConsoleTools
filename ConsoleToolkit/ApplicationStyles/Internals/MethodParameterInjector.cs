using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal class MethodParameterInjector
    {
        private List<object> _injectableReferences;

        public MethodParameterInjector(IEnumerable<object> injectableReferences)
        {
            _injectableReferences = injectableReferences.ToList();
        }

        public object[] GetParameters(MethodInfo method, IEnumerable<object> oneTimeInjectables)
        {
            var localInjectables = oneTimeInjectables.ToList();
            var injected = new List<object>();
            var parameters = method.GetParameters();
            foreach (var parameterInfo in parameters)
            {
                var value = localInjectables.FirstOrDefault(i => Matches(parameterInfo.ParameterType, i.GetType()))
                            ?? _injectableReferences.FirstOrDefault(i => Matches(parameterInfo.ParameterType, i.GetType()));

                if (value == null)
                {
                    throw new ParameterTypeCannotBeResolved(parameterInfo.ParameterType);
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
            return _injectableReferences.Any(i => Matches(parameterType, i.GetType()));
        }
    }
}