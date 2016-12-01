using System;
using System.Collections.Generic;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.TestingUtilities
{
    internal class FakeApplication : FakeApplicationBase
    {
        private readonly Type[] _commands;

        private FakeApplication(IConsoleAdapter console, IErrorAdapter error, Type[] commands) : base(console, error)
        {
            _commands = commands;
        }

        #region Overrides of ConsoleApplicationBase

        protected override void Initialise()
        {
            Config = new CommandLineInterpreterConfiguration();
            Handlers = new Dictionary<Type, ICommandHandler>();

            foreach (var type in _commands)
            {
                Config.Load(type);
                foreach(var handler in CommandHandlerLoader.LoadHandlerMethods(type, _commands, Injector.Value))
                    Handlers[handler.CommandType] = handler;
            }
            base.Initialise();
        }
        internal override void LoadConfigFromAssembly()
        {
        }

        #endregion

        public static FakeApplication MakeFakeApplication(IConsoleAdapter console, IErrorAdapter error, params Type[] commands)
        {
            var fake = new FakeApplication(console, error, commands);
            return fake;
        }
    }
}