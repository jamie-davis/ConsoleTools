using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class PosixCommandLineParser : ICommandLineParser, IOptionNameHelpAdorner
    {
        public void Parse(string[] args, IEnumerable<IOption> options, IEnumerable<IPositionalArgument> positionalArguments, IParserResult result)
        {
            var allowOptions = true;
            var optionList = options.ToList();

            var shortOptions = optionList.Where(o => o.Name.Length == 1).ToList(); //these need "-"
            var longOptions = optionList.Where(o => o.Name.Length > 1).ToList();  // these need "--"

            var argQueue = new Queue<string>();

            foreach (var arg in args)
                argQueue.Enqueue(arg);
            while (argQueue.Count > 0)
            {
                ParseOutcome outcome;
                var arg = argQueue.Dequeue();
                if (allowOptions && arg.StartsWith("--"))
                {
                    var optionName = arg.Substring(2);
                    if (optionName == string.Empty)
                    {
                        allowOptions = false;
                        continue;
                    }

                    outcome = ProcessLongOption(longOptions, optionName, argQueue, result);
                }
                else if (allowOptions && arg.StartsWith("-"))
                    outcome = ProcessShortOption(shortOptions, arg.Substring(1), argQueue, result);
                else
                    outcome = result.PositionalArgument(arg);

                if (outcome == ParseOutcome.Halt)
                    return;
            }
        }

        private ParseOutcome ProcessShortOption(List<IOption> shortOptions, string arg, Queue<string> argQueue, IParserResult result)
        {
            var option = shortOptions.FirstOrDefault(o => o.Name == GetShortOptionName(arg));
            if (option != null)
            {
                if (option.ParameterCount == 0 || option.IsBoolean)
                {
                    foreach (var optionChar in arg)
                    {
                        if (result.OptionExtracted(optionChar.ToString(CultureInfo.InvariantCulture), new string[] {}) == ParseOutcome.Halt)
                            return ParseOutcome.Halt;
                    }
                    return ParseOutcome.Continue;
                }
                
                if (arg.Length > 1)
                    return result.OptionExtracted(GetShortOptionName(arg), GetShortOptionArg(arg));

                if (ParameterAvailable(argQueue))
                    return result.OptionExtracted(GetShortOptionName(arg), new string[] { });

                string[] optionArgs;
                if (argQueue.Count > 0)
                    optionArgs = argQueue.Dequeue().Split(',');
                else
                    optionArgs = new string[] {};
                return result.OptionExtracted(option.Name, optionArgs);
            }

            if (arg.Length > 1)
                return result.OptionExtracted(GetShortOptionName(arg), GetShortOptionArg(arg));

            return result.OptionExtracted(arg, new string[] { });
        }

        private static string[] GetShortOptionArg(string arg)
        {
            return arg.Substring(1).Split(',');
        }

        private static string GetShortOptionName(string arg)
        {
            return arg.Substring(0, 1);
        }

        private ParseOutcome ProcessLongOption(List<IOption> longOptions, string arg, Queue<string> argQueue, IParserResult result)
        {
            string optionName;
            var equalPos = arg.IndexOf('=');
            if (equalPos < 0)
            {
                optionName = arg;

                var noEqualOption = longOptions.FirstOrDefault(o => o.Name == optionName);
                if (noEqualOption != null && noEqualOption.ParameterCount > 0 && ParameterAvailable(argQueue))
                {
                    var args = noEqualOption.ParameterCount == 1 ? new [] { argQueue.Dequeue() } : argQueue.Dequeue().Split(',');
                    return result.OptionExtracted(optionName, args);
                }

                return result.OptionExtracted(optionName, new string[] { });
            }

            optionName = arg.Substring(0, equalPos);
            var option = longOptions.FirstOrDefault(o => o.Name == optionName);
            var valuePart = arg.Substring(equalPos + 1);
            var optionArgs = option != null && option.ParameterCount == 1 ? new []{ valuePart } : valuePart.Split(',');
            return result.OptionExtracted(optionName, optionArgs);
        }

        private static bool ParameterAvailable(Queue<string> argQueue)
        {
            return argQueue.Count > 0 && !argQueue.Peek().StartsWith("-");
        }

        /// <summary>
        /// Add either "-" or "--" to an option name.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>The adjusted name.</returns>
        string IOptionNameHelpAdorner.Adorn(string name)
        {
            if (name.Length == 1)
                return "-" + name;
            return "--" + name;
        }
    }
}