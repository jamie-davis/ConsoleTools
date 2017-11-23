using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.Exceptions;

namespace ConsoleToolkit.ApplicationStyles
{
    /// <summary>
    /// This class provides the logic for validating that command handlers can be called. The idea is that this class is employed
    /// at an appropriate point to uncover any uncallable handlers. Becuase the validation can be applied to a full set of command
    /// handlers, it will enable errors made in command definition to be discovered as soon as possible, rather than relying on a 
    /// user to attempt to call the command.
    /// </summary>
    internal static class HandlerParameterValidator
    {
        /// <summary>
        /// Ensure that all parameters can be supplied using the provided injector. If any cannot, an exception is thrown.
        /// </summary>
        /// <param name="handlers">The command handler definitions to validate.</param>
        /// <param name="injector">The injector that will later be used to generate parameter values.</param>
        internal static void ValidateHandlers(IEnumerable<ICommandHandler> handlers, MethodParameterInjector injector)
        {
            foreach (var method in handlers.Where(m => m.HandlerMethod != null))
            {
                if (method.HandlerMethod.GetParameters().Any(p => p.ParameterType != method.CommandType && !injector.CanSupply(p.ParameterType)))
                    throw new CommandHandlerMethodHasUnsupportedParameter(method.CommandType, method.HandlerMethod);
            }
        }
    }
}
