using System;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.TestingUtilities
{
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
            var command = commandLineInterpreter.Interpret(args, out errors, false);
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

                app._helpHandler.Execute(app, null, app.Console, app.Injector.Value, CommandExecutionMode.CommandLine);
                return;
            }

            ExecuteCommand(app, command, CommandExecutionMode.CommandLine);
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
}