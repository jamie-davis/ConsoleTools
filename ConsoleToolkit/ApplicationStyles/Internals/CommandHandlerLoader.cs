using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal static class CommandHandlerLoader
    {
        private class MethodCommandTypeTuple
        {
            public MethodInfo Method;
            public Type HandlerType;
            public Type CommandType;
        };
        
        public static ICommandHandler Load(Type type, Type[] commandTypes, MethodParameterInjector injector)
        {
            var handlerAttribute = type.GetCustomAttribute<CommandHandlerAttribute>();
            if (handlerAttribute == null)
                throw new CommandHandlerDoesNotHaveAttribute(type);

            var handlerMethods = handlerAttribute.CommandType == null 
                ? GetHandlersForCommandTypes(type, commandTypes, injector) 
                : GetHandlersForSpecifiedType(type, handlerAttribute, injector);

            if (handlerMethods.Count == 1)
            {
                var handlerMethod = handlerMethods[0];
                return MakeHandler(handlerMethod.Method, handlerMethod.HandlerType, handlerMethod.CommandType);
            }

            if (handlerMethods.Count == 0)
                throw new NoCommandHandlerMethodFound(type);

            throw new AmbiguousCommandHandler(type);
        }

        private static List<MethodCommandTypeTuple> GetHandlersForSpecifiedType(Type type, CommandHandlerAttribute handlerAttribute, MethodParameterInjector injector)
        {
            return type.GetMethods()
                .Select(m => new {Method = m, Params = m.GetParameters()})
                .Where(p => IsValidCommandHandler(handlerAttribute.CommandType, p.Params, injector))
                .Select(p => new MethodCommandTypeTuple{ Method = p.Method, HandlerType = p.Method.DeclaringType, CommandType = handlerAttribute.CommandType})
                .ToList();
        }

        private static List<MethodCommandTypeTuple> GetHandlersForCommandTypes(Type type, Type[] commandTypes, MethodParameterInjector injector)
        {
            return type.GetMethods()
                .Select(m => new {Method = m, Params = m.GetParameters()})
                .Where(p => commandTypes.Any(ct => IsValidCommandHandler(ct, p.Params, injector)))
                .Select(p => new MethodCommandTypeTuple { Method = p.Method, HandlerType = p.Method.DeclaringType, CommandType = FindCommandTypeInMethodParameters(commandTypes, p.Method)})
                .ToList();
        }

        private static bool IsValidCommandHandler(Type commandType, ParameterInfo[] parameters, MethodParameterInjector injector)
        {
            return parameters.Any(p => p.ParameterType == commandType) 
                   && parameters.All(p => p.ParameterType == commandType || injector.CanSupply(p.ParameterType));
        }

        public static IEnumerable<ICommandHandler> LoadHandlerMethods(Type type, Type[] commandTypes, MethodParameterInjector injector)
        {
            var selfHandlerMethods = GetSelfHandlerMethods(type, commandTypes, injector).ToList();
            var externalHandlerMethods = GetHandlersForCommandTypes(type, commandTypes, injector);
            var handlerMethods = selfHandlerMethods.Concat(externalHandlerMethods)    
                .GroupBy(g => g.CommandType).ToList();
            var ambiguous = handlerMethods.FirstOrDefault(m => m.Count() > 1);
            if (ambiguous != null)
            {
                throw new MultipleHandlersForCommand(ambiguous.Key);
            }

            foreach (var handlerMethod in handlerMethods.SelectMany(h => h))
            {
                yield return MakeHandlerMethod(handlerMethod);
            }
        }

        private static IEnumerable<MethodCommandTypeTuple> GetSelfHandlerMethods(Type type, 
            Type[] commandTypes, MethodParameterInjector injector)
        {
            if (!commandTypes.Contains(type)) yield break;

            var methods = type.GetMethods().Where(m => m.GetCustomAttribute<CommandHandlerAttribute>() != null);
            var enumerator = methods.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                    yield break;
                else
                {
                    var method = enumerator.Current;
                    if (method.GetParameters().Any(p => !injector.CanSupply(p.ParameterType)))
                        throw new CommandHandlerMethodHasUnsupportedParameter(type, method);
                    
                    yield return new MethodCommandTypeTuple { Method = method, CommandType = type, HandlerType = type};
                }

                if (enumerator.MoveNext())
                    throw new CommandsMayOnlyDeclareOneHandlerMethod(type);
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static Type FindCommandTypeInMethodParameters(Type[] commandTypes, MethodInfo handlerMethod)
        {
            var commandParam =
                handlerMethod.GetParameters().FirstOrDefault(p => commandTypes.Contains(p.ParameterType));
            if (commandParam != null)
                return commandParam.ParameterType;

            return null;
        }

        private static ICommandHandler MakeHandlerMethod(MethodCommandTypeTuple handler)
        {
            if (handler.CommandType != handler.HandlerType) 
                return new CommandHandlerMethod(handler.Method, handler.CommandType);

            return new SelfHandler(handler.Method);
        }

        private static ICommandHandler MakeHandler(MethodInfo handlerMethod, Type handlerClass, Type commandType)
        {
            if (handlerClass != commandType && handlerClass.GetConstructor(Type.EmptyTypes) == null)
                throw new CommandHandlerMustHaveDefaultConstructor(handlerClass);

            var instance = Activator.CreateInstance(handlerClass);
            return new CommandHandler(instance, handlerMethod, commandType);
        }
    }
}