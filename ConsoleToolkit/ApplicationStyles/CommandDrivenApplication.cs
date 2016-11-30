﻿using System;
using System.Linq;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Exceptions;
using ConsoleToolkit.Properties;

namespace ConsoleToolkit.ApplicationStyles
{
    /// <summary>
    /// This is the base class that defines a command driven console application.<para/>
    /// A command driven console application supports a number of different commands that
    /// are identified with a keyword. Each command has it's own set of parameters and 
    /// options:<para/>
    /// <code>
    /// commandName [options] [parameters]
    /// </code>
    /// <para/>
    /// 
    /// Commands must be defined by an attribute based system. By default, the commands 
    /// supported by the application will be determined by searching the assembly containing
    /// the application for classes that have the <see cref="CommandAttribute"/>.
    /// </summary>
    public abstract class CommandDrivenApplication : ConsoleApplicationBase
    {
        private Func<object, string> _helpCommandParameterGetter;
        private Type _helpCommandType;
        private HelpHandler _helpHandler;

        [UsedImplicitly]
        protected static void Run(CommandDrivenApplication app, string[] args, IConsoleAdapter console, IErrorAdapter errorAdapter)
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

        private static void ConfigureHelpHandler(CommandDrivenApplication app, CommandLineInterpreter commandLineInterpreter)
        {
            if (app._helpHandler == null)
                app._helpHandler = new HelpHandler(null, null, app.Config);

            app._helpHandler.Adorner = commandLineInterpreter.GetOptionNameAdorner();
        }

        internal override void LoadConfigFromAssembly()
        {
            var types = GetCommandTypes();

            Config = new CommandLineInterpreterConfiguration(Toolkit.Options.ParsingConventions);
            foreach (var type in types)
                Config.Load(type);
        }

        protected override void PostInitialise()
        {
            base.PostInitialise();

            if (_helpCommandType != null)
            {
                if (!Config.Commands.Any(c => c.CommandType == _helpCommandType))
                    throw new HelpCommandMustBePartOfConfiguration();

                _helpHandler = new HelpHandler(_helpCommandType, _helpCommandParameterGetter, Config);
                Handlers.Add(_helpCommandType, _helpHandler);
            }
        }

        /// <summary>
        /// Call this method to supply the framework with the command type that should provide help to the user.
        /// The specified command type will be handled automatically by the framework to display usage text,
        /// and specified command help text.
        /// </summary>
        /// <typeparam name="T">The command type. This must be a type used only as the help command.</typeparam>
        /// <param name="getCommandParam">Supply a lambda that returns the command on which help is required. Return null to indicate that program level help.</param>
        protected void HelpCommand<T>(Func<T, string> getCommandParam)
        {
            _helpCommandType = typeof (T);
            _helpCommandParameterGetter = o => getCommandParam((T)o);
        }
    }
}
