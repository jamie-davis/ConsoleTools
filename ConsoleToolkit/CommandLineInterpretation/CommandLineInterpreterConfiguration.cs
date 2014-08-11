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
            var commandConfig = new CommandConfig<T>(w => new T()) { Name = command.ToLower(), CommandType = typeof(T)};
            _commands.Add(commandConfig);
            return commandConfig;
        }

        /// <summary>
        /// Specifies the default command (i.e. the command that gets all of the parameters if this is not a command oriented configuration).
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
        /// Specifies the default command (i.e. the command that gets all of the parameters if this is not a command oriented configuration).
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
        /// Specifies the default command (i.e. the command that gets all of the parameters if this is not a command oriented configuration).
        /// 
        /// Use this when your interface has no commands. It allows all of the options of a command to be specified, but does not need a command
        /// word to be specified. (i.e. the usual mode for console applications.)
        /// </summary>
        /// <typeparam name="T">The type that collects the parameters.</typeparam>
        /// <param name="applicationName">Specifies the application name. This is used only in usage reporting when the configuration <see cref="Describe"/> is called. The value specified here will be placed in the <see cref="ApplicationName"/> property.</param>
        public CommandConfig<T> Parameters<T>(string applicationName = null) where T : class, new ()
        {
            ApplicationName = applicationName;
            return Parameters(() => new T());
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
        /// Load the configuration of a command from a type.
        /// </summary>
        /// <param name="type">The type to load.</param>
        public void Load(Type type)
        {
            _commands.Add(CommandAttributeLoader.Load(type));
        }
    }
}