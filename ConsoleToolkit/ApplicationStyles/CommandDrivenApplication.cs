using System;
using System.Linq;
using ConsoleToolkit.Annotations;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

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
        protected static void Run(CommandDrivenApplication app, string[] args, IConsoleAdapter console)
        {
            app.Console = console;
            app.Initialise();
            app.PostInitialise();

            var commandLineInterpreter = new CommandLineInterpreter(app.Config);
            string[] errors;
            var command = commandLineInterpreter.Interpret(args, out errors);
            if (command == null)
            {
                foreach (var error in errors)
                {
                    app.Console.WrapLine(error);
                }
                Environment.ExitCode = app.CommandLineErrorExitCode;
                return;
            }

            if (app._helpHandler != null)
                app._helpHandler.Adorner = commandLineInterpreter.GetOptionNameAdorner();

            ExecuteCommand(app, command);
        }

        private static void ExecuteCommand(CommandDrivenApplication app, object command)
        {
            ICommandHandler handler;
            if (app.Handlers.TryGetValue(command.GetType(), out handler))
            {
                handler.Execute(app, command, app.Console, app.Injector.Value);
            }
            else
            {
                app.Console.WrapLine("No command handler found.");
                Environment.ExitCode = app.MissingCommandHandlerExitCode;
            }
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

        private class HelpHandler : ICommandHandler
        {
            private Func<object, string> _parameterGetter;
            private CommandLineInterpreterConfiguration _config;

            public HelpHandler(Type helpCommandType, Func<object, string> helpCommandParameterGetter, CommandLineInterpreterConfiguration config)
            {
                CommandType = helpCommandType;
                _parameterGetter = helpCommandParameterGetter;
                _config = config;
            }

            public Type CommandType { get; private set; }
            internal IOptionNameHelpAdorner Adorner { get; set; }

            public void Execute(ConsoleApplicationBase app, object command, IConsoleAdapter console, MethodParameterInjector injector)
            {
                var parameter = _parameterGetter(command);
                if (string.IsNullOrWhiteSpace(parameter))
                {
                    CommandDescriber.Describe(_config, console, DefaultApplicationNameExtractor.Extract(app.GetType()), Adorner);
                }
                else
                {
                    var chosenCommand = _config.Commands.FirstOrDefault(c => string.CompareOrdinal(c.Name, parameter) == 0);
                    if (chosenCommand == null)
                    {
                        console.WrapLine(@"The command ""{0}"" is not supported.");
                    }
                    else
                    {
                        CommandDescriber.Describe(chosenCommand, console, Adorner);
                    }
                }
            }
        }
    }

    public class MultipleCommandsInvalid : Exception
    {
        public MultipleCommandsInvalid() : base("ConsoleApplication derived classes must not have multiple commands defined.")
        {
            
        }
    }
}
