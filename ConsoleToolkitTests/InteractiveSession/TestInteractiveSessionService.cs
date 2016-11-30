using System;
using System.Collections.Generic;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.InteractiveSession;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.InteractiveSession
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestInteractiveSessionService
    {
        #region Types for test

        [Command("start")]
        public class StartCommand
        {
            [CommandHandler]
            public void Handle(IInteractiveSessionService service, IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
                service.BeginSession();
            }
        }

        [Command("one")]
        public class Command1
        {
            [CommandHandler]
            public void Handle(IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
            }
        }

        [Command("two")]
        public class Command2
        {
            [CommandHandler]
            public void Handle(IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
            }
        }

        [Command("three")]
        public class Command3
        {
            [CommandHandler]
            public void Handle(IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
            }
        }

        [Command("exit")]
        public class ExitCommand
        {
            [CommandHandler]
            public void Handle(IInteractiveSessionService service, IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
                service.EndSession();
            }
        }

        #endregion

        private ConsoleInterfaceForTesting _testConsole;
        private IConsoleAdapter _console;
        private IErrorAdapter _error;
        private FakeApplication _app;

        [SetUp]
        public void SetUp()
        {
            _testConsole = new ConsoleInterfaceForTesting();
            _console = new ConsoleAdapter(_testConsole);
            _error = new ErrorAdapter(_testConsole, "ERROR: ");
            _app = FakeApplication.MakeFakeApplication(_console, _error, typeof(StartCommand), typeof(Command1), typeof(Command2), typeof(Command3), typeof(ExitCommand));
        }

        [Test]
        public void InteractiveSessionRunsCommands()
        {
            //Arrange
            const string data = @"one
two
exit";
            using (var s = new StringReader(data))
            {
                _testConsole.SetInputStream(s);

                //Act
                UnitTestAppRunner.Run(_app, new [] { "start" }, _testConsole);

                //Assert
                _console.WrapLine("<Session exited>");
                Approvals.Verify(_testConsole.GetBuffer());
            }
        }

    }

    internal abstract class FakeApplicationBase : ConsoleApplicationBase
    {

        private HelpHandler _helpHandler;
        
        protected FakeApplicationBase(IConsoleAdapter console, IErrorAdapter error)
        {
            Console = console;
            Error = error;
        }


        protected static void Run(FakeApplicationBase app, string[] args, IConsoleAdapter console,
            IErrorAdapter errorAdapter)
        {
            app.Console = console;
            app.Error = errorAdapter;
            app.Initialise();
            app.PostInitialise();

            var commandLineInterpreter = new CommandLineInterpreter(app.Config);

            ConfigureHelpHandler(app, commandLineInterpreter);

            string[] errors;
            var command = commandLineInterpreter.Interpret(args, out errors);
            if (command == null)
            {
                if (errors != null)
                {
                    foreach (var error in errors)
                    {
                        app.Error.WrapLine(error);
                    }
                    Environment.ExitCode = app.CommandLineErrorExitCode;
                    return;
                }

                app._helpHandler.Execute(app, null, app.Console, app.Injector.Value);
                return;
            }

            ExecuteCommand(app, command);
        }

        private static void ConfigureHelpHandler(FakeApplicationBase app, CommandLineInterpreter commandLineInterpreter)
        {
            if (app._helpHandler == null)
                app._helpHandler = new HelpHandler(null, null, app.Config);

            app._helpHandler.Adorner = commandLineInterpreter.GetOptionNameAdorner();
        }
    

        #region Overrides of ConsoleApplicationBase

        protected override void OnCommandFailure()
        {
            Console.WrapLine("OnCommandFailure called.");
            base.OnCommandFailure();
        }

        protected override void OnCommandSuccess()
        {
            Console.WrapLine("OnCommandSuccess called.");
            base.OnCommandSuccess();
        }

        #endregion
    }


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