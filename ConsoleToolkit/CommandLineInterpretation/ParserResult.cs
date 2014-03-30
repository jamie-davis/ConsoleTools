using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public enum ParseOutcome
    {
        Continue,
        Halt
    }
    public class ParserResult : IParserResult
    {
        private List<CommandLineInterpreterConfiguration.BasePositional> _positionals;
        private Dictionary<string, CommandLineInterpreterConfiguration.BaseOption> _options;
        private List<CommandLineInterpreterConfiguration.BaseOption> _usedOptions;
        private List<string> _usedPositionals;

        public object ParamObject { get; set; }
        public string Error { get; private set; }

        public ParseStatus Status { get; private set; }

        public ParserResult(CommandLineInterpreterConfiguration.BaseCommandConfig command, string name)
        {
            _positionals = command.Positionals.ToList();
            _usedPositionals = new List<string>();
            _options = command.Options
                .SelectMany(o => new[] { OptionCollectionEntry(o) }.Concat(OptionAliases(o)))
                .ToDictionary(c => c.Key, c => c.Value);
            _usedOptions = new List<CommandLineInterpreterConfiguration.BaseOption>();
            ParamObject = command.Create(name);
            Status = ParseStatus.Incomplete;
        }

        private static IEnumerable<KeyValuePair<string, CommandLineInterpreterConfiguration.BaseOption>> OptionAliases(CommandLineInterpreterConfiguration.BaseOption o)
        {
            return o.Aliases.Select(
                a => new KeyValuePair<string, CommandLineInterpreterConfiguration.BaseOption>(a, o));
        }

        private static KeyValuePair<string, CommandLineInterpreterConfiguration.BaseOption> OptionCollectionEntry(CommandLineInterpreterConfiguration.BaseOption o)
        {
            return new KeyValuePair<string, CommandLineInterpreterConfiguration.BaseOption>(o.Name, o);
        }

        public ParseOutcome OptionExtracted(string optionName, string[] arguments)
        {
            CommandLineInterpreterConfiguration.BaseOption option;
            if (_options.TryGetValue(optionName, out option))
            {
                if (_usedOptions.Contains(option))
                {
                    Error = string.Format("The \"{0}\" option may only be specified once.", option.Name);
                    return ParseOutcome.Halt;
                }

                _usedOptions.Add(option);

                string error;
                option.Apply(ParamObject, arguments, out error);
                if (error != null)
                {
                    Error = error;
                    return ParseOutcome.Halt;
                }
                return option.IsShortCircuit ? ParseOutcome.Halt : ParseOutcome.Continue;
            }

            var positional = _positionals.FirstOrDefault(p => p.ParameterName == optionName);
            if (positional != null)
            {
                if (!AcceptPositional(arguments.First(), positional))
                    return ParseOutcome.Halt;
                return ParseOutcome.Continue;
            }

            if (_usedPositionals.Contains(optionName))
            {
                Error = string.Format("The \"{0}\" parameter may only be specified once.", optionName);
                return ParseOutcome.Halt;
            }

            Error = string.Format("\"{0}\" is not a valid option.", optionName);
            return ParseOutcome.Halt;
        }

        public ParseOutcome PositionalArgument(string value)
        {
            if (_positionals.Any())
            {
                return AcceptPositional(value, _positionals.First()) 
                    ? ParseOutcome.Continue 
                    : ParseOutcome.Halt;
            }

            Error = string.Format("Unexpected argument \"{0}\"", value);
            return ParseOutcome.Halt;
        }

        private bool AcceptPositional(string value, CommandLineInterpreterConfiguration.BasePositional positional)
        {
            _positionals.Remove(positional);
            _usedPositionals.Add(positional.ParameterName);

            var error = positional.Accept(ParamObject, value);
            if (error == null)
            {
                return true;
            }

            Error = error;
            return false;
        }

        public void ParseCompleted()
        {
            if (Error != null)
                Status = ParseStatus.Failed;
            else if (_positionals.Any() && !_usedOptions.Any(o => o.IsShortCircuit))
            {
                Error = "Not enough parameters specified.";
                Status = ParseStatus.Failed;
            }
            else
                Status = ParseStatus.CompletedOk;
        }
    }
}