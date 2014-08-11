using System;
using System.Reflection;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal class CommandHandlerMethodHasUnsupportedParameter : Exception
    {
        public Type HandlerType { get; private set; }
        public MethodInfo HandlerMethod { get; private set; }

        public CommandHandlerMethodHasUnsupportedParameter(Type handlerType, MethodInfo handlerMethod)
            : base(string.Format("The command handler type {0} declares a handler function {1} that has an invalid parameter type.", handlerType, handlerMethod.Name))
        {
            HandlerType = handlerType;
            HandlerMethod = handlerMethod;
        }
    }
}