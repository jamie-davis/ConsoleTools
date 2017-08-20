using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkit.ApplicationStyles
{
    internal class HelpHandler : ICommandHandler
    {
        private CommandLineInterpreterConfiguration _config;
        private Type _helpCommandType;
        private Func<object, ICollection<string>> _parametersGetter;
        private CommandLineInterpreterConfiguration config;

        public HelpHandler(Type helpCommandType, Func<object, object> helpCommandParameterGetter, CommandLineInterpreterConfiguration config)
        {
            CommandType = helpCommandType;
            _parametersGetter = o => MakeParameters(helpCommandParameterGetter(o));
            _config = config;
        }

        private ICollection<string> MakeParameters(object parameters)
        {
            if (parameters is ICollection<string>)
                return (ICollection<string>) parameters;

            if (parameters is string)
                return new List<string> { (string)parameters };

            return new List<string> { parameters.ToString() };
        }

        public Type CommandType { get; private set; }
        internal IOptionNameHelpAdorner Adorner { get; set; }

        public void Execute(ConsoleApplicationBase app, object command, IConsoleAdapter console, MethodParameterInjector injector, CommandExecutionMode executionMode)
        {
            var parameters = _parametersGetter == null || command == null ? null : _parametersGetter(command);

            if (parameters == null || parameters.Count == 0 || (parameters.Count == 1 && string.IsNullOrEmpty(parameters.First())))
            {
                CommandDescriber.Describe(_config, console, DefaultApplicationNameExtractor.Extract(app.GetType()), executionMode, Adorner);
            }
            else
            {
                var partialMatches = CommandSelector.FindPartialMatches(parameters, _config.Commands.Cast<ICommandKeys>().ToList());
                if (partialMatches.Count == 0)
                {
                    console.WrapLine($@"The command ""{string.Join(" ", parameters)}"" is not supported.");
                }
                else if (partialMatches.Count == 1)
                {
                    CommandDescriber.Describe((BaseCommandConfig) partialMatches[0], console, executionMode, Adorner);
                }
                else
                {
                    CommandDescriber.DescribeKeywords(partialMatches.Cast<BaseCommandConfig>(), parameters, console);
                }
            }
        }
    }
}