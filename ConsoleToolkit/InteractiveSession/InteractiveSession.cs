using System;
using System.Collections.Generic;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkit.InteractiveSession
{
    internal class InteractiveSession
    {
        private readonly ConsoleApplicationBase _app;
        private readonly MethodParameterInjector _injector;
        private readonly Dictionary<Type, ICommandHandler> _handlers;
        private readonly IConsoleAdapter _console;
        private readonly IErrorAdapter _error;
        private readonly CommandLineInterpreterConfiguration _config;
        private bool _stopped = false;
        private CommandLineInterpreter _interpreter;

        public InteractiveSession(ConsoleApplicationBase app, MethodParameterInjector injector, Dictionary<Type, ICommandHandler> handlers, IConsoleAdapter console, IErrorAdapter error, CommandLineInterpreterConfiguration config)
        {
            _app = app;
            _injector = injector;
            _handlers = handlers;
            _console = console;
            _error = error;
            _config = config;
            _interpreter = new CommandLineInterpreter(_config);
        }

        public string Prompt { get; set; }

        public void Run()
        {
            _stopped = false;
            while (!_stopped)
            {
                if (Prompt != null)
                    _console.Wrap(Prompt);

                var commandText = _console.ReadLine();
                if (commandText == null)
                    return;

                var tokens = CommandLineTokeniser.Tokenise(commandText);
                string[] errors;

                var command = _interpreter.Interpret(tokens, out errors, false, true);
                if (command == null)
                {
                    foreach (var error in errors)
                        _error.WrapLine(error);
                    continue;
                }

                HandleCommand(command);
            }
        }

        private void HandleCommand(object command)
        {
            ConsoleApplicationBase.ExecuteCommand(_app, command, CommandExecutionMode.Interactive);
        }

        public void Stop()
        {
            _stopped = true;
        }
    }
}