using System.Collections.Generic;
using System.Linq;
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

        public object Interpret(string[] args, out string[] errors)
        {
            var interpreter = GetInterpreter();

            var messages = new List<string>();

            BaseCommandConfig command;
            string commandName = null;
            int firstArgumentIndex;
            if (_config.Commands.Any())
            {
                if (args == null || args.Length == 0)
                {
                    errors = null;
                    return null;
                }

                commandName = args[0].ToLower();
                command = _config.Commands.FirstOrDefault(c => c.Name == commandName);
                if (command == null)
                {
                    messages.Add("Command not recognised.");
                    errors = messages.ToArray();
                    return null;
                }
                firstArgumentIndex = 1;
            }
            else
            {
                if (_config.DefaultCommand == null)
                    throw new CommandConfigurationInvalid();
                command = _config.DefaultCommand;
                firstArgumentIndex = 0;
            }

            var parserArgs = args.Skip(firstArgumentIndex).ToArray();
            var result = new ParserResult(command, commandName);
            var optionList = GetOptionsAndAliases(command);
            interpreter.Parse(parserArgs, optionList, command.Positionals, result);
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

        /// <summary>
        /// Get the full list of ways each option can be referenced. i.e. by its actual name, and all of its aliases.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A list of IOption implementations.</returns>
        private IEnumerable<IOption> GetOptionsAndAliases(BaseCommandConfig command)
        {
            return command
                .Options
                .SelectMany(o => new IOption[] {o}
                                    .Concat(o.Aliases.Select(a => new OptionAlias(o, a))));
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
