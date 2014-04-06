using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// Fluent command configuration construction object.
    ///
    /// Either the default command, or at least one named command must be configured. 
    /// See <see cref="Parameters{T}(System.Func{T})"/>"/>, and <see cref="Command{T}(string)"/> overloads.
    /// </summary>
    public class CommandLineInterpreterConfiguration
    {
        public ICommandLineParser CustomParser { get; private set; }
        public CommandLineParserConventions ParserConvention { get; set; }

        internal static readonly Dictionary<Type, Func<string, object>> Converters = new Dictionary<Type, Func<string, object>>
        {
            {typeof(bool), TryParse<bool>},
            {typeof(short), TryParse<short>},
            {typeof(ushort), TryParse<ushort>},
            {typeof(int), TryParse<int>},
            {typeof(uint), TryParse<uint>},
            {typeof(long), TryParse<long>},
            {typeof(ulong), TryParse<ulong>},
            {typeof(float), TryParse<float>},
            {typeof(double), TryParse<double>},
            {typeof(decimal), TryParse<decimal>},
            {typeof(byte), TryParse<byte>},
            {typeof(char), TryParse<char>},
            {typeof(DateTime), TryParse<DateTime>},
            {typeof(string), v => v},
        };

        private static object TryParse<T>(string value)
        {
            try
            {
                var type = typeof(T);
                return TypeDescriptor.GetConverter(type)
                    .ConvertFromInvariantString(value);
            }
            catch
            {
                return null;
            }
        }

        public CommandLineInterpreterConfiguration() : this(CommandLineParserConventions.MicrosoftStandard)
        {
            
        }

        public CommandLineInterpreterConfiguration(CommandLineParserConventions parserConvention)
        {
            ParserConvention = parserConvention;
            ValidateParserConvention();
        }

        private void ValidateParserConvention()
        {
            if (ParserConvention == CommandLineParserConventions.CustomConventions && CustomParser == null)
                throw new CustomParserNotFound();
        }

        public CommandLineInterpreterConfiguration(ICommandLineParser customParser)
        {
            CustomParser = customParser;
            ParserConvention = CommandLineParserConventions.CustomConventions;
        }

        public static void AddCustomConverter<T>(Func<string, T> converter)
        {
            Converters[typeof (T)] = s => converter(s);
        }
        
        private List<BaseCommandConfig> _commands = new List<BaseCommandConfig>();

        public IEnumerable<BaseCommandConfig> Commands { get { return _commands; } }

        /// <summary>
        /// Adds a command to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of the command that is created.</typeparam>
        /// <param name="command">The name of the command i.e. the string that the user will enter to identify the command.</param>
        /// <param name="initialiser">A function that creates and initialises the command object.</param>
        public CommandConfig<T> Command<T>(string command, Func<string, T> initialiser) where T : class
        {
            if (DefaultCommand != null)
                throw new NamedCommandConflict();

            if (_commands.Any(c => string.Compare(c.Name, command, true, CultureInfo.CurrentCulture) == 0))
                throw new CommandAlreadySpecified(command);
            var commandConfig = new CommandConfig<T>(initialiser) { Name = command.ToLower() };
            _commands.Add(commandConfig);
            return commandConfig;
        }

        /// <summary>
        /// Adds a command to the configuration, with an auto generated initialiser. This is only possible where T has a parameterless constructor.
        /// </summary>
        /// <typeparam name="T">The type of the command that is created. Must have a parameterless constructor to be used with this overload.</typeparam>
        /// <param name="command">The name of the command i.e. the string that the user will enter to identify the command.</param>
        public CommandConfig<T> Command<T>(string command) where T : class, new()
        {
            if (DefaultCommand != null)
                throw new NamedCommandConflict();

            if (_commands.Any(c => string.Compare(c.Name, command, true, CultureInfo.CurrentCulture) == 0))
                throw new CommandAlreadySpecified(command);
            var commandConfig = new CommandConfig<T>(w => new T()) { Name = command.ToLower() };
            _commands.Add(commandConfig);
            return commandConfig;
        }

        /// <summary>
        /// Specifies the default command (i.e. the command that gets all of the parameters if the first one is not a recognised command).
        /// 
        /// Use this when your interface has no commands. It allows all of the options of a command to be specified, but does not need a command
        /// word to be specified. (i.e. the usual mode for console applications.)
        /// </summary>
        /// <typeparam name="T">The type that collects the parameters.</typeparam>
        /// <param name="initialiser">A function that creates and initialises the command object (the <see cref="T"/> instance).</param>
        public CommandConfig<T> Parameters<T>(Func<T> initialiser) where T : class
        {
            if (DefaultCommand != null)
                throw new ProgramParametersAlreadySpecified();

            var commandConfig = new CommandConfig<T>(initialiser) { Name = null };
            DefaultCommand = commandConfig;
            return commandConfig;
        }

        /// <summary>
        /// Specifies the default command (i.e. the command that gets all of the parameters if the first one is not a recognised command).
        /// 
        /// Use this when your interface has no commands. It allows all of the options of a command to be specified, but does not need a command
        /// word to be specified. (i.e. the usual mode for console applications.)
        /// </summary>
        /// <typeparam name="T">The type that collects the parameters.</typeparam>
        /// <param name="applicationName">Specifies the application name. This is used only in usage reporting when the configuration <see cref="Describe"/> is called. The value specified here will be placed in the <see cref="ApplicationName"/> property.</param>
        /// <param name="initialiser">A function that creates and initialises the command object (the <see cref="T"/> instance).</param>
        public CommandConfig<T> Parameters<T>(string applicationName, Func<T> initialiser) where T : class
        {
            ApplicationName = applicationName;
            return Parameters(initialiser);
        }

        /// <summary>
        /// The name of the application. This will be used by <see cref="Describe"/> when formatting the program's parameter description.
        /// If no value is specified, the executing assembly's name will be retrieved and used.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The default command. If this is set, this will be used when no commands are configured. Either the default command,
        /// or at least one named command must be configured. See <see cref="Parameters{T}(System.Func{T})"/>"/>, and <see cref="Command{T}(string)"/> overloads.
        /// </summary>
        public BaseCommandConfig DefaultCommand { get; set; }

        /// <summary>
        /// Generate the usage text for the configuration. This will be formatted to fit into the specified width. Some elements have minimum widths, so very narrow settings of <see cref="consoleWidth"/> will not be respected.
        /// </summary>
        /// <param name="consoleWidth">The desired width of the descriptive text. This will be respected as far as possible, but some output elements have minimum widths so it may not always be possible to obey the setting consistently.</param>
        /// <returns>A string containing the formatted usage text.</returns>
        public string Describe(int consoleWidth)
        {
            var sb = new StringBuilder();

            if (DefaultCommand != null)
                AddDefaultCommandText(consoleWidth, sb, DefaultCommand);

            AddCommandListText(consoleWidth, sb);
            return sb.ToString();
        }

        private void AddDefaultCommandText(int consoleWidth, StringBuilder sb, BaseCommandConfig defaultCommand)
        {
            sb.Append(FormatCommandDescription(consoleWidth, defaultCommand, String.Format("Usage: {0}", ApplicationName ?? DefaultApplicationName())));
        }

        private void AddCommandListText(int consoleWidth, StringBuilder sb)
        {
            var commands = Commands.Where(c => c.Name != null).ToList();
            if (commands.Any())
            {
                var maxWidth = commands.Max(c => c.Name.Length) + 2;
                var textWidth = Math.Max(consoleWidth - maxWidth, 10);
                var actualWidth = maxWidth + textWidth;

                TextFormatter.AppendWidth(sb, actualWidth, "Available commands");
                sb.AppendLine();

                foreach (var command in commands)
                {
                    var commandText = FormatCommandDescription(textWidth, command);

                    var commandName = TextFormatter.FormatBlock(maxWidth, command.Name);
                    sb.Append(TextFormatter.MergeBlocks(commandName, maxWidth, commandText));
                    sb.AppendLine();
                }
            }
        }

        private string DefaultApplicationName()
        {
            var trace = new StackTrace();
            var index = 0;
            string bestName = null;
            while (index < trace.FrameCount)
            {
                var frame = trace.GetFrame(index++);
                var method = frame.GetMethod();
                if (method.DeclaringType != null)
                {
                    var assembly = method.DeclaringType.Assembly;
                    if (method.Name == "Main")
                    {
                        if (method.DeclaringType != null) 
                            return assembly.GetName().Name;
                    }
                    else
                    {
                        if (bestName == null && assembly != GetType().Assembly)
                            bestName = assembly.GetName().Name;
                    }
                }
            }
            return bestName;
        }

        private string FormatCommandDescription(int textWidth, BaseCommandConfig command, string prefixText = null)
        {
            var sb = new StringBuilder();
            TextFormatter.AppendWidth(sb, textWidth, command.Description ?? String.Empty);

            var positionalsPresent = command.Positionals.Any();
            var optionsPresent = command.Options.Any();
            if (positionalsPresent || optionsPresent)
            {
                sb.AppendLine();

                var paramList = !positionalsPresent ? String.Empty :
                    " " + command.Positionals.Select(p => String.Format("<{0}>", p.ParameterName))
                        .Aggregate((t, i) => t + " " + i);
                var options = !optionsPresent ? String.Empty : " [options]";
                sb.Append(TextFormatter.FormatBlock(textWidth, String.Format("{0}{1}{2}{3}", prefixText ?? String.Empty, command.Name, paramList, options)));
            }

            if (positionalsPresent)
            {
                sb.AppendLine();
                sb.Append(TextFormatter.FormatBlock(textWidth, "Parameters:"));
                sb.AppendLine();

                var maxParameterWidth = command.Positionals.Max(p => p.ParameterName.Length) + 2;
                var paramDescWidth = Math.Max(10, textWidth - maxParameterWidth);

                foreach (var positional in command.Positionals)
                {
                    var paramName = TextFormatter.FormatBlock(maxParameterWidth, positional.ParameterName);
                    var paramDesc = TextFormatter.FormatBlock(paramDescWidth, positional.Description ?? String.Empty);

                    sb.Append(TextFormatter.MergeBlocks(paramName, maxParameterWidth, paramDesc));
                }                
            }

            if (optionsPresent)
            {
                sb.AppendLine();
                sb.Append(TextFormatter.FormatBlock(textWidth, "Options:"));
                sb.AppendLine();

                var maxOptionWidth = command.Options.Max(p => p.Name.Length) + 3;
                var optionDescWidth = Math.Max(10, textWidth - maxOptionWidth);

                foreach (var option in command.Options)
                {
                    var optionName = TextFormatter.FormatBlock(maxOptionWidth, "/" + option.Name);
                    var optionDesc = TextFormatter.FormatBlock(optionDescWidth, option.Description);

                    sb.Append(TextFormatter.MergeBlocks(optionName, maxOptionWidth, optionDesc));
                }                
            }

            return sb.ToString();
        }
    }
}