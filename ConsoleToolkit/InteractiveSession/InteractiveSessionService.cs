using System;
using System.Collections.Generic;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkit.InteractiveSession
{
    internal class InteractiveSessionService : IInteractiveSessionService
    {
        private MethodParameterInjector _injector;
        private CommandLineInterpreterConfiguration _config;
        private Dictionary<Type, ICommandHandler> _handlers;
        private ConsoleApplicationBase _app;
        private readonly IConsoleAdapter _console;
        private readonly IErrorAdapter _error;
        private bool _inProgress;
        private InteractiveSession _activeSession;
        private string _prompt;

        public InteractiveSessionService(IConsoleAdapter console, IErrorAdapter error)
        {
            _console = console;
            _error = error;
        }

        public void Initialise(ConsoleApplicationBase app, MethodParameterInjector injector, CommandLineInterpreterConfiguration config, Dictionary<Type, ICommandHandler> handlers)
        {
            _app = app;
            _injector = injector;
            _config = config;
            _handlers = handlers;
        }

        #region Implementation of IInteractiveSessionService

        public void BeginSession()
        {
            _inProgress = true;
            _activeSession = new InteractiveSession(_app, _injector, _handlers, _console, _error, _config) { Prompt = _prompt };
            _activeSession.Run();
        }

        public void EndSession()
        {
            _activeSession.Stop();
        }

        public void SetPrompt(string prompt)
        {
            _prompt = prompt;
            if (_activeSession != null)
                _activeSession.Prompt = _prompt;
        }

        #endregion
    }
}