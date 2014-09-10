using System;
using System.Linq;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public static class CommandDescriber
    {
        private const string ColumnSeperator = "  ";
        private const ReportFormattingOptions FormattingOptions = ReportFormattingOptions.OmitHeadings;

        public static void Describe(CommandLineInterpreterConfiguration config, IConsoleAdapter console, string applicationName, IOptionNameHelpAdorner adorner = null)
        {
            if (config.DefaultCommand != null)
                AddDefaultCommandText(console, config.DefaultCommand, applicationName, adorner);
            else
                AddCommandListText(console, config, adorner);
        }

        private static void AddDefaultCommandText(IConsoleAdapter console, BaseCommandConfig defaultCommand, string applicationName, IOptionNameHelpAdorner adorner)
        {
            console.Write(FormatFullCommandDescription(defaultCommand, string.Format("Usage: {0}", applicationName), adorner));
        }

        private static void AddCommandListText(IConsoleAdapter console, CommandLineInterpreterConfiguration config, IOptionNameHelpAdorner adorner)
        {
            var commands = config.Commands.Where(c => c.Name != null).ToList();
            if (commands.Any())
            {
                console.WriteLine("Available commands");
                console.WriteLine();
                var commandItems = commands.Select(c => new { Command = c.Name, Text = FormatShortCommandDescription(c) });
                console.FormatTable(commandItems, FormattingOptions, ColumnSeperator);
            }
        }

        private static IConsoleRenderer FormatFullCommandDescription(BaseCommandConfig command, string prefixText = null, IOptionNameHelpAdorner adorner = null)
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
                formatter.WrapLine(string.Format("{0}{1}{2}{3}", prefixText ?? String.Empty, command.Name, paramList, options));
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

        public static void Describe(BaseCommandConfig command, IConsoleAdapter console, IOptionNameHelpAdorner adorner)
        {
            console.Write(FormatFullCommandDescription(command, adorner: adorner));
        }
    }
}
