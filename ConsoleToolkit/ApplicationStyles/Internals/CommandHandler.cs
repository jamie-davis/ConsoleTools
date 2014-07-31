using System;
using System.Reflection;
using ConsoleToolkit.ConsoleIO;

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

        public void Execute(ConsoleApplicationBase app, object command, IConsoleAdapter console, MethodParameterInjector injector)
        {
            Execute(command, injector);
        }

        private void Execute(object command, MethodParameterInjector injector)
        {
            try
            {
                var parameters = injector.GetParameters(_method, new [] {command});
                _method.Invoke(_handler, parameters);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }
    }
}