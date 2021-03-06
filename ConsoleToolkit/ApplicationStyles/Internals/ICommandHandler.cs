﻿using System;
using System.Reflection;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    interface ICommandHandler
    {
        Type CommandType { get; }
        void Execute(ConsoleApplicationBase app, object command, IConsoleAdapter console, MethodParameterInjector injector, CommandExecutionMode executionMode);
        MethodInfo HandlerMethod { get; }
    }
}
