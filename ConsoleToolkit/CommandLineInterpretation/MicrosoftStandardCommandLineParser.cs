using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class MicrosoftStandardCommandLineParser : ICommandLineParser, IOptionNameHelpAdorner
    {
        public void Parse(string[] args, IEnumerable<IOption> optionsEnumerable, IEnumerable<IPositionalArgument> positionalArgumentsEnumerable, IParserResult result)
        {
            var allowOptions = true;
            var options = optionsEnumerable.ToList();
            var positionals = positionalArgumentsEnumerable.ToList();
            var argQueue = new Queue<string>();
            foreach (var arg in args)
            {
                argQueue.Enqueue(arg);
            }

            while (argQueue.Count > 0)
            {
                var arg = argQueue.Dequeue();
                ParseOutcome outcome;
                if (allowOptions && arg.StartsWith("-"))
                {
                    if (arg == "--")
                    {
                        allowOptions = false;
                        continue;
                    }
                    outcome = GetOption(result, argQueue, arg.Substring(1), options, positionals);
                }
                else
                    outcome = result.PositionalArgument(arg);

                if (outcome == ParseOutcome.Halt) return;
            }
        }

        private ParseOutcome GetOption(IParserResult result, Queue<string> argQueue, string arg, List<IOption> options, List<IPositionalArgument> positionals)
        {
            var embeddedOptionName = GetOptionName(arg);
            var option = options.FirstOrDefault(o => string.Compare(o.Name, embeddedOptionName, true) == 0);
            string optionName;
            string[] optionArgs;
            if (option != null)
            {
                optionName = option.Name;
                optionArgs = ExtractOptionArgs(arg, argQueue, option);
            }
            else
            {
                var pos = positionals.FirstOrDefault(p => string.Compare(p.ParameterName, embeddedOptionName, true) == 0);
                if (pos != null)
                    optionName = pos.ParameterName;
                else
                    optionName = embeddedOptionName;
                optionArgs = ExtractOptionArgs(arg, argQueue, null);
            }

            return result.OptionExtracted(optionName, optionArgs);
        }

        private string GetOptionName(string arg)
        {
            var colonPos = arg.IndexOf(':');
            if (colonPos < 0)
                return arg;

            return arg.Substring(0, colonPos);
        }

        private string[] ExtractOptionArgs(string input, Queue<string> argQueue, IOption option)
        {
            var colonPos = input.IndexOf(':');
            if (colonPos <= 0)
            {
                string[] arguments;
                if (option != null && option.ParameterCount > 0 && argQueue.Count > 0 && !argQueue.Peek().StartsWith("-"))
                    arguments = argQueue.Dequeue().Split(',');
                else
                    arguments = new string[] {};

                return arguments;
            }

            return input.Substring(colonPos + 1).Split(',');
        }

        /// <summary>
        /// Add "-" to an option name.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>The adjusted name.</returns>
        string IOptionNameHelpAdorner.Adorn(string name)
        {
            return "-" + name;
        }

    }
}
