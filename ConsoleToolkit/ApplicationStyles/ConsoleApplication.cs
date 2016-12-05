using System;
using System.Data;
using System.Linq;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Exceptions;

namespace ConsoleToolkit.ApplicationStyles
{
    /// <summary>
    /// This is the base class that defines a console application.<para/>
    /// A console application has a single set of parameters and options defined by the
    /// default command.<para/>
    /// The application's assembly will be searched for a command class identified by the 
    /// <see cref="CommandAttribute"/>.
    /// </summary>
    public abstract class ConsoleApplication : ConsoleApplicationBase
    {
        private Func<object, bool> _helpOptionGetter;
        private Type _helpCommandType;

        protected static void Run(ConsoleApplication app, string[] args, IConsoleAdapter console, IErrorAdapter errorAdapter)
        {
            app.Console = console;
            app.Error = errorAdapter;
            app.Initialise();
            app.PostInitialise();
            ValidateHelpSettings(app);
            ProcessCommandLine(app, args);
        }

        private static void ValidateHelpSettings(ConsoleApplication app)
        {
            if (app._helpCommandType != null && app.Config.DefaultCommand.CommandType != app._helpCommandType)
            {
                throw new HelpCommandMustBePartOfConfiguration();
            }
        }

        private static void ProcessCommandLine(ConsoleApplication app, string[] args)
        {
            var commandLineInterpreter = new CommandLineInterpreter(app.Config);
            var optionNameHelpAdorner = commandLineInterpreter.GetOptionNameAdorner();

            if (args.Length == 0 && AllowNoArgsHelpText(app))
            {
                app.DisplayHelp(optionNameHelpAdorner);
                Environment.ExitCode = app.CommandLineErrorExitCode;
                return;
            }

            string[] errors;
            var command = commandLineInterpreter.Interpret(args, out errors, true);
            if (command == null)
            {
                foreach (var error in errors)
                {
                    app.Error.WrapLine(error);
                }
                Environment.ExitCode = app.CommandLineErrorExitCode;
                return;
            }

            HandleCommand(app, command, optionNameHelpAdorner);
        }

        private static bool AllowNoArgsHelpText(ConsoleApplication app)
        {
            return app.Config.DefaultCommand != null
                   && app.Config.DefaultCommand.Positionals.Any(p => !p.IsOptional);
        }

        private static void HandleCommand(ConsoleApplication app, object command, IOptionNameHelpAdorner optionNameAdorner)
        {
            if (AutoHelpConfigured(app, command))
                app.DisplayHelp(optionNameAdorner);
            else
                ExecuteCommand(app, command);
        }

        private static void ExecuteHandler(ConsoleApplication app, object command)
        {
            var handler = app.Handlers.FirstOrDefault().Value;
            if (handler != null)
            {
                app.OnCommandLineValid(command);
                if (Environment.ExitCode == 0)
                    handler.Execute(app, command, app.Console, app.Injector.Value);
                RunPostCommandMethod(app);
            }
            else
            {
                app.Console.WrapLine("No command handler found.");
                Environment.ExitCode = app.MissingCommandHandlerExitCode;
            }
        }

        private static void RunPostCommandMethod(ConsoleApplication app)
        {
            var success = Environment.ExitCode == 0;
            if (success)
                app.OnCommandSuccess();
            else
                app.OnCommandFailure();
        }

        private static bool AutoHelpConfigured(ConsoleApplication app, object command)
        {
            return app._helpOptionGetter != null && app._helpOptionGetter(command);
        }

        private void DisplayHelp(IOptionNameHelpAdorner optionNameAdorner)
        {
            var appName = DefaultApplicationNameExtractor.Extract(GetType());
            CommandDescriber.Describe(Config, Console, appName, optionNameAdorner);
        }

        protected void HelpOption<T>(Func<T, bool> isHelp)
        {
            _helpCommandType = typeof(T);
            _helpOptionGetter = c =>
                                    {
                                        if (c is T)
                                            return isHelp((T)c);
                                        return false;
                                    };
        }

        internal override void LoadConfigFromAssembly()
        {
            var commandTypes = GetCommandTypes(CommandScanType.NonInteractiveCommands).ToList();
            if (commandTypes.Count > 1) 
                throw new MultipleCommandsInvalid();

            if (commandTypes.Count == 0)
                throw new ConsoleApplicationRequiresDefaultCommand();

            Config = new CommandLineInterpreterConfiguration(Toolkit.Options.ParsingConventions)
                         {
                             DefaultCommand = CommandAttributeLoader.Load(commandTypes.Single())
                         };
            foreach (var interactiveCommand in GetCommandTypes(CommandScanType.InteractiveCommands))
            {
                Config.Load(interactiveCommand);
            }
        }
    }
}