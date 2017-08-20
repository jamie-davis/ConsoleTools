using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public static class CommandDescriber
    {
        private const string ColumnSeperator = "  ";
        private const ReportFormattingOptions FormattingOptions = ReportFormattingOptions.OmitHeadings;

        public static void Describe(CommandLineInterpreterConfiguration config, IConsoleAdapter console, string applicationName, CommandExecutionMode executionMode, IOptionNameHelpAdorner adorner = null)
        {
            if (config.DefaultCommand != null && executionMode == CommandExecutionMode.CommandLine)
                AddDefaultCommandText(console, config.DefaultCommand, applicationName, adorner);
            else
                AddCommandListText(console, config, adorner, executionMode);
        }

        private static void AddDefaultCommandText(IConsoleAdapter console, BaseCommandConfig defaultCommand, string applicationName, IOptionNameHelpAdorner adorner)
        {
            console.Write(FormatFullCommandDescription(defaultCommand, string.Format("Usage: {0}", applicationName), adorner, false));
        }

        private static void AddCommandListText(IConsoleAdapter console, CommandLineInterpreterConfiguration config, IOptionNameHelpAdorner adorner, CommandExecutionMode executionMode)
        {
            var relevantCommands = config.Commands.Where(c => c.Name != null && CommandModeFilter(executionMode, c)).ToList();
            var keywordCommands = relevantCommands
                .Where(c => c.Keywords.Count > 0)
                .Select(c => new KeywordCommand(c.Keywords, c.Name, c.KeywordsDocs))
                .ToList();
            var keywordHelpText = KeywordCommandHelpFormatter.FormatKeywordList(keywordCommands);
            var commands = relevantCommands.Where(c => c.Keywords.Count == 0).ToList();
            if (commands.Any() || keywordHelpText.Any())
            {
                console.WriteLine("Available commands");
                console.WriteLine();
                var commandItems = commands.Select(c => new CommandDescription { Command = c.Name, Text = FormatShortCommandDescription(c) })
                    .Concat(keywordHelpText)
                    .OrderBy(c => c.Command);
                console.FormatTable(commandItems, FormattingOptions, ColumnSeperator);
            }
        }

        private static bool CommandModeFilter(CommandExecutionMode executionMode, BaseCommandConfig baseCommandConfig)
        {
            if (executionMode == CommandExecutionMode.CommandLine)
                return baseCommandConfig.ValidInNonInteractiveContext;

            if (executionMode == CommandExecutionMode.Interactive)
                return baseCommandConfig.ValidInInteractiveContext;

            return false;
        }

        private static IConsoleRenderer FormatFullCommandDescription(BaseCommandConfig command, string prefixText = null, IOptionNameHelpAdorner adorner = null, bool displayCommandName = true)
        {
            var formatter = new RecordingConsoleAdapter();
            formatter.WrapLine(((IContext)command).Description ?? string.Empty);

            var positionalsPresent = command.Positionals.Any();
            var optionsPresent = command.Options.Any();
            if (positionalsPresent || optionsPresent)
            {
                formatter.WriteLine();

                var paramList = !positionalsPresent ? String.Empty :
                    " " + command.Positionals.Select(FormatParameterListEntry)
                        .Aggregate((t, i) => t + " " + i);
                var options = !optionsPresent ? String.Empty : " [options]";
                formatter.WrapLine(string.Format("{0}{1}{2}{3}", prefixText ?? String.Empty, displayCommandName ? command.Name : null, paramList, options));
            }

            if (positionalsPresent)
            {
                formatter.WriteLine();
                formatter.WriteLine("Parameters:");
                formatter.WriteLine();
                var positionals = command.Positionals
                    .Select(p => new {p.ParameterName, Description = FormatPositionalDescription(p)});
                formatter.FormatTable(positionals, FormattingOptions, ColumnSeperator);
            }

            if (optionsPresent)
            {
                formatter.WriteLine();
                formatter.WriteLine("Options:");
                formatter.WriteLine();

                var options = command.Options
                    .Select(o => new { OptionName = GetOptionNameAndAliases(adorner, o), o.Description });
                formatter.FormatTable(options, FormattingOptions, ColumnSeperator);
            }

            return formatter;
        }

        private static string FormatShortCommandDescription(BaseCommandConfig command, string prefixText = null)
        {
            return ((IContext)command).Description ?? string.Empty;
        }

        private static string GetOptionNameAndAliases(IOptionNameHelpAdorner adorner, BaseOption option)
        {
            var names = new [] { option.Name }.Concat(option.Aliases);
            return string.Join(", ", names.Select(n => adorner == null ? n : adorner.Adorn(n)));
        }

        private static string FormatPositionalDescription(BasePositional positional)
        {
            if (positional.Description == null) return string.Empty;

            if (positional.IsOptional && positional.DefaultValue != null)
                return string.Format("{0} Default value: {1}", positional.Description, positional.DefaultValue);
            return positional.Description;
        }

        private static string FormatParameterListEntry(BasePositional p)
        {
            var entry = String.Format("<{0}>", p.ParameterName);
            return p.IsOptional ? string.Format("[{0}]", entry) : entry;
        }

        public static void Describe(BaseCommandConfig command, IConsoleAdapter console, CommandExecutionMode executionMode, IOptionNameHelpAdorner adorner)
        {
            console.Write(FormatFullCommandDescription(command, adorner: adorner));
        }

        public static void DescribeKeywords(IEnumerable<BaseCommandConfig> commands, ICollection<string> parameters, IConsoleAdapter console)
        {
            var doc = GetKeywordsDocs(commands, parameters);
            if (doc != null)
            {
                console.WrapLine(doc.Description);
                console.WriteLine();
            }

            var matchedCommands = commands
                .Select(c => new { Words = c.Keywords.Concat(new[] { c.Name }).ToList(), Command = c })
                .Where(d => d.Words.Count > parameters.Count)
                .Where(d => d.Words.Zip(parameters, (k, p) => string.Compare(k, p, StringComparison.InvariantCultureIgnoreCase) == 0).All(c => c))
                .Select(d => d.Command);

            var nextWords = matchedCommands
                .Select(c => c.Keywords.Concat(new [] { c.Name }).ToList())
                .Where(c => c.Count > parameters.Count)
                .Select(c => c[parameters.Count])
                .Distinct()
                .OrderBy(c => c);

            var validContinuations = nextWords
                .Select(w => parameters.Concat(new[] {w}).ToList())
                .Select(w => new {Command = string.Join(" ", w), Desc = GetKeywordsDocs(commands, w)})
                .Select(d => new { d.Command, Desc = d.Desc == null ? string.Empty : d.Desc.Description ?? string.Empty});

            var report = validContinuations
                .AsReport(rep => rep.OmitHeadings()
                    .AddColumn(c => c.Command, cc => { })
                    .AddColumn(c => '-', cc => { })
                    .AddColumn(c => c.Desc, cc => { }));

            console.FormatTable(report);

        }

        private static KeywordsDesc GetKeywordsDocs(IEnumerable<BaseCommandConfig> commands, ICollection<string> parameters)
        {
            var keywordsDoc = commands.SelectMany(c => c.KeywordsDocs)
                .Where(d => d.Keywords.Count == parameters.Count && !string.IsNullOrWhiteSpace(d.Description))
                .FirstOrDefault(d => d.Keywords.Zip(parameters, (k, p) => string.Compare(k, p, StringComparison.InvariantCultureIgnoreCase) == 0).All(c => c));

            if (keywordsDoc == null)
            {
                var commandDoc = commands
                    .Select(c => new { AllWords = c.Keywords.Concat(new [] {c.Name}).ToList(), Desc = ((IContext)c).Description })
                    .Where(d => d.AllWords.Count == parameters.Count && !string.IsNullOrWhiteSpace(d.Desc))
                    .FirstOrDefault(d => d.AllWords.Zip(parameters, (k, p) => string.Compare(k, p, StringComparison.InvariantCultureIgnoreCase) == 0).All(c => c));
                if (commandDoc != null)
                    return new KeywordsDesc(commandDoc.Desc, commandDoc.AllWords);
            }

            return keywordsDoc;
        }
    }
}
