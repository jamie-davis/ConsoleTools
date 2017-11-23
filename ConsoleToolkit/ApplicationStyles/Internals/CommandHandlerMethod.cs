using System;
using System.Reflection;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal class CommandHandlerMethod : ICommandHandler
    {
        public Type CommandType { get { return _commandType; } }
        public void Execute(ConsoleApplicationBase app, object command, IConsoleAdapter console, MethodParameterInjector injector, CommandExecutionMode executionMode)
        {
            Execute(app, command, injector);
        }

        public MethodInfo HandlerMethod => _method;

        private readonly MethodInfo _method;
        private readonly Type _commandType;

        internal CommandHandlerMethod(MethodInfo method, Type commandType)
        {
            _method = method;
            _commandType = commandType;
        }

        internal void Execute(ConsoleApplicationBase app, object command, MethodParameterInjector injector)
        {
            try
            {
                var parameters = injector.GetParameters(_method, new[] { command });
                MethodInvoker.Invoke(_method, app, parameters);
            }
            catch (Exception e)
            {
                Toolkit.HandleException(e, command, injector);
            }
        }
    }

    internal class SelfHandler : ICommandHandler
    {
        private readonly MethodInfo _method;

        public SelfHandler(MethodInfo method)
        {
            _method = method;
            CommandType = method.DeclaringType;
        }

        public Type CommandType { get; private set; }

        public void Execute(ConsoleApplicationBase app, object command, IConsoleAdapter console, MethodParameterInjector injector, CommandExecutionMode executionMode)
        {
            try
            {
                var parameters = injector.GetParameters(_method, new[] { command });
                MethodInvoker.Invoke(_method, command, parameters);
            }
            catch (Exception e)
            {
                Toolkit.HandleException(e, command, injector);
            }
        }

        public MethodInfo HandlerMethod => _method;
    }
}