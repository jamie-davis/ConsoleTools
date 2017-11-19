using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.Exceptions;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// This class interprets a set of command line arguments.
    /// </summary>
    public sealed class CommandLineInterpreter
    {
        private readonly CommandLineInterpreterConfiguration _config;

        public CommandLineInterpreter(CommandLineInterpreterConfiguration config)
        {
            _config = config;
        }

        public object Interpret(string[] args, out string[] errors, bool useDefault, bool isInteractive = false)
        {
            var interpreter = GetInterpreter();

            var messages = new List<string>();

            BaseCommandConfig command = null;
            string commandName = null;
            var firstArgumentIndex = 0;

            if (useDefault || !_config.Commands.Any())
            {
                if (_config.DefaultCommand == null)
                    throw new CommandConfigurationInvalid();
                command = _config.DefaultCommand;
                firstArgumentIndex = 0;
            }
            else
            {
                if (args == null || args.Length == 0)
                {
                    errors = null;
                    return null;
                }

                var selection = CommandSelector.Select(args, _config.Commands.Cast<ICommandKeys>().ToList());
                if (!selection.Matched)
                {
                    errors = new[] {selection.Error};
                    return null;
                }

                command = (BaseCommandConfig)selection.Command;
                commandName = command.Name.ToLower();
                firstArgumentIndex = selection.UsedArgumentCount;
            }

            if (command == null)
            {
                messages.Add("Command not recognised.");
                errors = messages.ToArray();
                return null;
            }

            var parserArgs = args.Skip(firstArgumentIndex).ToArray();
            var executionMode = isInteractive ? CommandExecutionMode.Interactive : CommandExecutionMode.CommandLine;
            var result = new ParserResult(command, commandName, _config, executionMode);
            var options = GetOptionsAndAliases(result.GetAllOptions()).ToList();

            interpreter.Parse(parserArgs, options, command.Positionals, result);
            result.ParseCompleted();
            if (result.Status != ParseStatus.CompletedOk)
            {
                errors = new[] {result.Error};
                return null;
            }

            var validationMessages = new List<string>();
            var commandValid = command.Validate(result.ParamObject, validationMessages);

            messages.AddRange(validationMessages);
            errors = messages.ToArray();

            return commandValid ? result.ParamObject : null;
        }

        /// <summary>
        /// Get the full list of ways each option can be referenced.
        /// </summary>
        /// <param name="options">All of the options with their reference name.</param>
        /// <returns>A list of IOption implementations.</returns>
        private IEnumerable<IOption> GetOptionsAndAliases(IEnumerable<KeyValuePair<string, BaseOption>> options)
        {
            return options
                .Select(o => new OptionAlias(o.Value, o.Key));
        }


        /// <summary>
        /// An alias for an option. This references the original option by one of its alias names.
        /// This is returned as part of an enumerable set of <see cref="IOption"/> implementations.
        /// </summary>
        private class OptionAlias : IOption
        {
            private readonly BaseOption _option;
            private readonly string _alias;
            public string Name { get { return _alias; } }
            public bool IsBoolean { get { return _option.IsBoolean; } }
            public int ParameterCount { get { return _option.ParameterCount; } }

            public OptionAlias(BaseOption option, string alias)
            {
                _option = option;
                _alias = alias;
            }
        }

        private ICommandLineParser GetInterpreter()
        {
            if (_config.ParserConvention == CommandLineParserConventions.CustomConventions)
            {
                if (_config.CustomParser != null)
                    return _config.CustomParser;

                throw new CustomParserNotFound();
            }

            return BuiltInCommandLineParsers.Get(_config.ParserConvention);
        }

        public IOptionNameHelpAdorner GetOptionNameAdorner()
        {
            var interpreter = GetInterpreter();
            if (interpreter is IOptionNameHelpAdorner)
                return (IOptionNameHelpAdorner) interpreter;

            return null;
        }
    }
}
