using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ApplicationStyles.Internals;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal class OptionsAnalyser
    {
        private readonly BaseCommandConfig _command;
        private readonly CommandLineInterpreterConfiguration _config;

        internal class AnalysedOption
        {
            private List<string> _validNames;

            public BaseOption BaseOption { get; }

            public IEnumerable<string> ValidNames => _validNames;

            internal AnalysedOption(List<string> validNames, BaseOption baseOption)
            {
                BaseOption = baseOption;
                _validNames = validNames;
            }
        }

        public OptionsAnalyser(BaseCommandConfig command, CommandLineInterpreterConfiguration config)
        {
            _command = command;
            _config = config;
        }

        private List<BaseOption> GetCommandOptions(CommandExecutionMode executionMode)
        {
            var commandOptionsSource = _command.Options.AsEnumerable();
            if (executionMode == CommandExecutionMode.CommandLine && _config?.GlobalOptions != null && _config.GlobalOptions.Any())
            {
                commandOptionsSource = commandOptionsSource.Concat(_config.GlobalOptions.SelectMany(g => g.Options));
            }

            var commandOptions = commandOptionsSource.ToList();
            return commandOptions;
        }

        public bool Any(CommandExecutionMode executionMode)
        {
            return GetCommandOptions(executionMode).Any();
        }

        public IEnumerable<AnalysedOption> AllOptions(CommandExecutionMode executionMode)
        {
            var commandOptions = GetCommandOptions(executionMode);
            var usedOptionNames = new List<string>();
            foreach (var commandOption in commandOptions)
            {
                var nameValid = !usedOptionNames.Contains(commandOption.Name);
                usedOptionNames.Add(commandOption.Name);

                var validAliases = commandOption.Aliases.Where(n => !usedOptionNames.Contains(n)).ToList();
                usedOptionNames.AddRange(validAliases);

                var validNames = new List<string>();
                if (nameValid)
                    validNames.Add(commandOption.Name);

                validNames.AddRange(validAliases);

                if (validNames.Any())
                    yield return new AnalysedOption(validNames, commandOption);
            }
        }
    }
}
