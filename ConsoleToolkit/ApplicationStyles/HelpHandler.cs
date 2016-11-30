using System;
using System.Linq;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkit.ApplicationStyles
{
    internal class HelpHandler : ICommandHandler
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
            var parameter = _parameterGetter == null || command == null ? String.Empty : _parameterGetter(command);
            if (String.IsNullOrWhiteSpace(parameter))
            {
                CommandDescriber.Describe(_config, console, DefaultApplicationNameExtractor.Extract(app.GetType()), Adorner);
            }
            else
            {
                var chosenCommand = _config.Commands.FirstOrDefault(c => String.CompareOrdinal(c.Name, parameter) == 0);
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