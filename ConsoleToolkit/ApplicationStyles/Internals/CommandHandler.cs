using System;
using System.Reflection;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    internal class CommandHandler : ICommandHandler
    {
        public Type CommandType { get { return _commandType; } }

        private readonly object _handler;
        private readonly MethodInfo _method;
        private readonly Type _commandType;

        internal CommandHandler(object handler, MethodInfo method, Type commandType)
        {
            _handler = handler;
            _method = method;
            _commandType = commandType;
        }

        public void Execute(ConsoleApplicationBase app, object command, IConsoleAdapter console, MethodParameterInjector injector, CommandExecutionMode executionMode)
        {
            Execute(command, injector);
        }

        public MethodInfo HandlerMethod => _method;

        private void Execute(object command, MethodParameterInjector injector)
        {
            try
            {
                var parameters = injector.GetParameters(_method, new [] {command});
                MethodInvoker.Invoke(_method, _handler, parameters);
            }
            catch (Exception e)
            {
                Toolkit.HandleException(e, command, injector);
            }
        }
    }
}