using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// Fluent command configuration construction object.
    ///
    /// Either the default command, or at least one named command must be configured. 
    /// See <see cref="Parameters{T}(System.Func{T})"/>"/>, and <see cref="Command{T}"/>.
    /// </summary>
    public class CommandLineInterpreterConfiguration
    {
        public ICommandLineParser CustomParser { get; private set; }
        public CommandLineParserConventions ParserConvention { get; set; }

        private static readonly Dictionary<Type, Func<string, object>> Converters = new Dictionary<Type, Func<string, object>>
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

        public interface IContext
        {
            string Description { get; set; }
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

            if (_commands.Any(c => string.Compare(c.Name, command, true) == 0))
                throw new CommandAlreadySpecified(command);
            var commandConfig = new CommandConfig<T>(initialiser) { Name = command.ToLower() };
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
        /// or at least one named command must be configured. See <see cref="Parameters{T}(System.Func{T})"/>"/>, and <see cref="Command{T}"/>.
        /// </summary>
        public BaseCommandConfig DefaultCommand { get; set; }

        /// <summary>
        /// The base class for command configurations.
        /// </summary>
        public abstract class BaseCommandConfig : IContext
        {
            public string Name { get; set; }
            public List<BasePositional> Positionals = new List<BasePositional>();
            public List<BaseOption> Options = new List<BaseOption>();

            internal abstract object Create(string commandName);
            internal abstract bool Validate(object command, IList<string> messages);
            public string Description { get; set; }
        }

        /// <summary>
        /// The configuration for a command. This may be the unnamed command (i.e. the command line parameters for an application that does not support commands)
        /// or a named "sub-command" of a program that supports that paradigm.
        /// </summary>
        /// <typeparam name="T">The type that will be populated with the command parameters extracted from the command line.</typeparam>
        public class CommandConfig<T> : BaseCommandConfig where T :class
        {
            private readonly Func<string, T> _initialiser;
            private IContext _currentContext;
            private Func<T, IList<string>, bool> _validator;

            public CommandConfig(Func<T> initialiser) : this(s => initialiser())
            {
            }

            public CommandConfig(Func<string, T> initialiser)
            {
                _initialiser = initialiser;
                _currentContext = this;
            }

            /// <summary>
            /// A positional parameter. The framework must be able to convert the user's input string from the command line into the specific type that this parameter accepts (<see cref="TParameter"/>)
            /// </summary>
            /// <typeparam name="TParameter">The parameter type. The text supplied by the user will be converted to this type by the framework.</typeparam>
            public class CommandPositional<TParameter> : BasePositional
            {
                private readonly Action<T, TParameter> _positionalInitialiser;
                private Func<string, object> _converter;

                public CommandPositional(string parameterName, Action<T, TParameter> positionalInitialiser)
                    : base(parameterName)
                {
                    _positionalInitialiser = positionalInitialiser;
                    if (!Converters.TryGetValue(typeof(TParameter), out _converter))
                        throw new InvalidParameterType(typeof(TParameter));
                }

                public override string Accept(object command, string value)
                {
                    var typeCastCommand = command as T;

                    var parameter = _converter(value);
                    if (parameter == null)
                        return String.Format("The {0} parameter value \"{1}\" is invalid.", ParameterName, value);
                    _positionalInitialiser(typeCastCommand, (TParameter)parameter);
                    return null;
                }
            }

            /// <summary>
            /// A command option. The framework must be able to convert the user's input strings from the command 
            /// line into the specific types that this option accepts.  The types are extracted from the 
            /// specification of the provided action (<see cref="TAction"/>).
            /// </summary>
            /// <typeparam name="TAction">The action that will be executed when the option is supplied with parameters that can be converted.</typeparam>
            public class CommandOption<TAction> : BaseOption
            {
                private readonly TAction _optionInitialiser;
                private List<Type> _paramTypes;
                private Type _commandType;
                private Type _actionType;

                public CommandOption(string optionName, TAction optionInitialiser)
                {
                    _optionInitialiser = optionInitialiser;
                    Name = optionName;

                    AnalyseAction();
                }

                private void AnalyseAction()
                {
                    _actionType = _optionInitialiser.GetType();
                    if (_actionType.IsGenericType)
                    {
                        var genericArguments = _actionType.GetGenericArguments();
                        _commandType = genericArguments.First();
                        _paramTypes = genericArguments.Skip(1).ToList();
                        ParameterCount = _paramTypes.Count();

                        var invalidParamType = _paramTypes.FirstOrDefault(t => t != typeof(string) && !Converters.ContainsKey(t));
                        if (invalidParamType != null)
                            throw new InvalidParameterType(invalidParamType);
                    }
                }

                public override void Apply(object command, IEnumerable<string> parameters, out string error)
                {
                    object[] callParameters;
                    if (CreateCallParameters(command, parameters, out callParameters, out error))
                        _actionType.GetMethod("Invoke").Invoke(_optionInitialiser, callParameters);
                    else if (error == null)
                        error = "Internal error parsing command.";
                }

                private bool CreateCallParameters(object command, IEnumerable<string> parameters, out object[] callParameters, out string error)
                {
                    error = null;

                    var convertedParameters = new List<object> {command};

                    using (var paramEnumerator = parameters.GetEnumerator())
                    {
                        var index = 0;
                        while (paramEnumerator.MoveNext())
                        {
                            var parameter = paramEnumerator.Current;

                            if (index >= _paramTypes.Count)
                            {
                                callParameters = null;
                                error = string.Format("The {0} option has too many parameters.", Name);
                                return false;
                            }


                            if (!AddParameter(_paramTypes[index++], ref convertedParameters, parameter))
                            {
                                callParameters = null;
                                error = string.Format("The parameter \"{0}\" of the {1} option has an invalid value.", parameter, Name);
                                return false;
                            }
                        }

                        if (IsBoolean && index == 0)
                        {
                            callParameters = new[] { command, true };
                            return true;
                        }

                        if (ParameterCount == 0)
                        {
                            callParameters = new [] { command };
                            return true;
                        }

                        if (index < ParameterCount)
                        {
                            callParameters = null;
                            error = string.Format("Not enough parameters for the {0} option.", Name);
                            return false;
                        }

                        callParameters = convertedParameters.ToArray();
                        return true;
                        
                    }
                }

                private bool AddParameter(Type paramType, ref List<object> parameterList, string parameterText)
                {
                    object value = null;
                    if (typeof (string) == paramType)
                        value = parameterText;
                    else
                    {
                        Func<string, object> converter;
                        if (Converters.TryGetValue(paramType, out converter))
                            value = converter(parameterText);
                    }
                    parameterList.Add(value);
                    return value != null;
                }
            }

            /// <summary>
            /// Add a positional parameter.
            /// </summary>
            /// <typeparam name="T1">The data type of the paramter value.</typeparam>
            /// <param name="parameterName">The name of the parameter</param>
            /// <param name="positionalInitialiser">An expression that sets the value of the parameter in the command type.</param>
            public CommandConfig<T> Positional<T1>(string parameterName, Action<T, T1> positionalInitialiser)
            {
                var commandPositional = new CommandPositional<T1>(parameterName, positionalInitialiser);
                Positionals.Add(commandPositional);
                _currentContext = commandPositional;
                return this;
            }

            /// <summary>
            /// Short circuit the parsing process. This allows options to be specified that bypass the usual validation of
            /// missing parameters. Options that display help text are a good example of where this could be useful.
            /// </summary>
            public CommandConfig<T> ShortCircuitOption()
            {
                if (_currentContext is BaseOption)
                {
                    (_currentContext as BaseOption).IsShortCircuit = true;
                    return this;
                }


                if (ContextIsPositional())
                    throw new ShortCircuitInvalidOnPositionalParameter(_currentContext);

                throw new ShortCircuitInvalid();
            }

            /// <summary>
            /// Specifies the simplest possible option type - there are no parameters, the option is simply present.
            /// However, some parsing conventions allow for a boolean to be specified allowing a false to be supplied.
            /// In order to support this, the option must accept a boolean and apply it appropriately.
            /// </summary>
            /// <param name="optionName">The name of the option.</param>
            /// <param name="optionInitialiser">The lambda that applies the option to the command parameters type. Note that this must accept a boolean.</param>
            /// <returns>The command config.</returns>
            public CommandConfig<T> Option(string optionName, Action<T, bool> optionInitialiser)
            {
                var commandOption = new CommandOption<Action<T, bool>>(optionName, optionInitialiser) { IsBoolean = true};
                Options.Add(commandOption);
                _currentContext = commandOption;
                return this;
            }

            /// <summary>
            /// Specifies an option taking a single parameter.
            /// </summary>
            /// <typeparam name="T1">The parameter.</typeparam>
            /// <param name="optionName">The name of the option.</param>
            /// <param name="optionInitialiser">The action that will be invoked when the option's parameters have been converted.</param>
            /// <returns>The command config.</returns>
            public CommandConfig<T> Option<T1>(string optionName, Action<T, T1> optionInitialiser)
            {
                var commandOption = new CommandOption<Action<T, T1>>(optionName, optionInitialiser);
                Options.Add(commandOption);
                _currentContext = commandOption;
                return this;
            }

            /// <summary>
            /// Specifies an option taking a two parameters.
            /// </summary>
            /// <typeparam name="T1">The first parameter.</typeparam>
            /// <typeparam name="T2">The second parameter.</typeparam>
            /// <param name="optionName">The name of the option.</param>
            /// <param name="optionInitialiser">The action that will be invoked when the option's parameters have been converted.</param>
            /// <returns>The command config.</returns>
            public CommandConfig<T> Option<T1, T2>(string optionName, Action<T, T1, T2> optionInitialiser)
            {
                var commandOption = new CommandOption<Action<T, T1, T2>>(optionName, optionInitialiser);
                Options.Add(commandOption);
                _currentContext = commandOption;
                return this;
            }

            /// <summary>
            /// Specifies an option taking a two parameters.
            /// </summary>
            /// <typeparam name="T1">The first parameter.</typeparam>
            /// <typeparam name="T2">The second parameter.</typeparam>
            /// <typeparam name="T3">The third parameter.</typeparam>
            /// <param name="optionName">The name of the option.</param>
            /// <param name="optionInitialiser">The action that will be invoked when the option's parameters have been converted.</param>
            /// <returns>The command config.</returns>
            public CommandConfig<T> Option<T1, T2, T3>(string optionName, Action<T, T1, T2, T3> optionInitialiser)
            {
                var commandOption = new CommandOption<Action<T, T1, T2, T3>>(optionName, optionInitialiser);
                Options.Add(commandOption);
                _currentContext = commandOption;
                return this;
            }

            internal override object Create(string commandName)
            {
                return _initialiser(commandName);
            }

            internal override bool Validate(object command, IList<string> messages)
            {
                if (_validator == null || command == null) return true;
                if (command is T) 
                    return _validator(command as T, messages);

                messages.Add(string.Format("Internal error: Command type received was {0}, but {1} was expected.", command.GetType(), typeof (T)));
                return false;
            }

            /// <summary>
            /// Use this method to provide descriptive text. The description is context sensitive and will be applied to the command, 
            /// option or parameter that is currently being configured. Therefore, you must specify the appropriate description before 
            /// configuring more detail.
            /// </summary>
            /// <param name="text">The descriptive test.</param>
            /// <returns>The command config.</returns>
            public CommandConfig<T> Description(string text)
            {
                if (_currentContext != null)
                    _currentContext.Description = text;
                return this;
            }

            /// <summary>
            /// Supply a validation routine for the command or parameters. This will be called with the populated command instance and
            /// a list into which error messages and warnings may be inserted. The validator should return true if the command is valid, 
            /// or false if an error is found. If any messages are placed in the list, they are assyumed to be warnings if true is
            /// returned, or errors if false is returned.
            /// 
            /// Error messages returned by the validator will be printed to the error writer, warnings will be printed to the console.
            /// </summary>
            /// <param name="validationFunction"></param>
            public void Validator(Func<T, IList<string>,  bool> validationFunction)
            {
                _validator = validationFunction;
            }

            /// <summary>
            /// Supply an alternative name for the option. All of the alias names and the primary option name will refer to the same option.
            /// This feature can be used to supply a short name for an option with a long name. This is common in Gnu command line applications.
            /// </summary>
            /// <param name="alias">The alternative name for the option.</param>
            public CommandConfig<T> Alias(string alias)
            {
                if (_currentContext is BaseOption)
                {
                    var existingNames = Options.SelectMany(o => new [] { o.Name }.Concat(o.Aliases));
                    (_currentContext as BaseOption).Alias(alias, existingNames);
                    return this;
                }

                throw new AliasNotSupported();
            }

            private bool ContextIsPositional()
            {
                return Positionals.Contains(_currentContext);
            }
        }

        /// <summary>
        /// The base class for positional parameters of command configurations.
        /// </summary>
        public abstract class BasePositional : IContext, IPositionalArgument
        {
            public string ParameterName { get; set; }

            protected BasePositional(string parameterName)
            {
                ParameterName = parameterName;
            }

            public abstract string Accept(object command, string value);
            public string Description { get; set; }
        }

        /// <summary>
        /// The base class for optional paramters of command configurations.
        /// </summary>
        public abstract class BaseOption : IContext, IOption
        {
            private List<string> _aliases = new List<string>();

            public IEnumerable<string> Aliases { get { return _aliases; } }
            public int ParameterCount { get; set; }
            public string Name { get; set; }

            public abstract void Apply(object command, IEnumerable<string> parameters, out string error);
            public string Description { get; set; }

            public bool IsBoolean { get; internal set; }

            public bool IsShortCircuit { get; set; }

            public void Alias(string alias, IEnumerable<string> existingOptionNames)
            {
                if (alias == Name || existingOptionNames.Contains(alias))
                    throw new DuplicateOptionName();
                _aliases.Add(alias);
            }
        }

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