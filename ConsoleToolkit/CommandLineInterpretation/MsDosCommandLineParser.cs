using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class MsDosCommandLineParser : ICommandLineParser, IOptionNameHelpAdorner
    {
        public void Parse(string[] args, IEnumerable<IOption> options, IEnumerable<IPositionalArgument> positionalArguments, IParserResult result)
        {
            var optionList = options as IOption[] ?? options.ToArray();
            var argQueue = new Queue<string>();
            foreach (var arg in args)
            {
                argQueue.Enqueue(arg);
            }

            while (argQueue.Count > 0)
            {
                var arg = argQueue.Dequeue();

                ParseOutcome outcome;
                if (arg.StartsWith("/"))
                    outcome = ProcessOption(optionList, arg, argQueue, result);
                else
                    outcome = result.PositionalArgument(arg);

                if (outcome == ParseOutcome.Halt)
                    return;
            }
        }

        private ParseOutcome ProcessOption(IEnumerable<IOption> options, string arg, Queue<string> argQueue, IParserResult result)
        {
            string[] optionArgs = null;
            var optionDetails = options as IList<IOption> ?? options.ToList();
            var optionNames = optionDetails.Select(o => o.Name);
            var colonPos = arg.IndexOf(':');
            if (colonPos < 0)
            {
                var optionName = GetOptionName(optionNames, arg.Substring(1));
                var optionDefinition = optionDetails.FirstOrDefault(o => System.String.Compare(o.Name, optionName, System.StringComparison.OrdinalIgnoreCase) == 0);
                if (optionDefinition != null)
                {
                    optionName = optionDefinition.Name;
                    optionArgs = ExtractOptionArgs(argQueue, optionDefinition);
                }

                return result.OptionExtracted(optionName, optionArgs ?? new string[] { });
            }

            var option = arg.Substring(1, colonPos - 1);
            optionArgs = arg.Substring(colonPos + 1).Split(',');
            return result.OptionExtracted(GetOptionName(optionNames, option), optionArgs);
        }

        private string[] ExtractOptionArgs(Queue<string> argQueue, IOption option)
        {
            string[] arguments;
            if (option != null && option.ParameterCount > 0 && argQueue.Count > 0 && !argQueue.Peek().StartsWith("/"))
            {
                if (option.IsBoolean && !IsBoolValue(argQueue.Peek()))
                    arguments = new string[] {};
                else
                    arguments = argQueue.Dequeue().Split(',');
            }
            else
                arguments = new string[] {};

            return arguments;
        }

        private bool IsBoolValue(string value)
        {
            bool notUsed;
            return bool.TryParse(value, out notUsed);
        }

        private static string GetOptionName(IEnumerable<string> optionNames, string optionName)
        {
            return optionNames.FirstOrDefault(n => System.String.Compare(optionName, n, System.StringComparison.OrdinalIgnoreCase) == 0) ?? optionName;
        }

        /// <summary>
        /// Add "/" to an option name.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>The adjusted name.</returns>
        string IOptionNameHelpAdorner.Adorn(string name)
        {
            return "/" + name;
        }
    }
}