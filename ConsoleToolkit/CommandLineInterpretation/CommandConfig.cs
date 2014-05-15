using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleToolkit.CommandLineInterpretation
{
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
        /// Add a positional parameter.
        /// </summary>
        /// <typeparam name="T1">The data type of the paramter value.</typeparam>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="positionalInitialiser">An expression that sets the value of the parameter in the command type.</param>
        public CommandConfig<T> Positional<T1>(string parameterName, Action<T, T1> positionalInitialiser)
        {
            var commandPositional = new CommandPositional<T, T1>(parameterName, positionalInitialiser);
            Positionals.Add(commandPositional);
            _currentContext = commandPositional;
            return this;
        }

        /// <summary>
        /// Add a positional parameter.
        /// </summary>
        /// <typeparam name="T1">The data type of the paramter value.</typeparam>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="positionalVariableIdentifier">An expression that returns the property in the command type that should receive the value.</param>
        public CommandConfig<T> Positional<T1>(string parameterName, Expression<Func<T, T1>> positionalVariableIdentifier)
        {
            var commandPositional = ConfigGenerator.PositionalFromExpression(parameterName, positionalVariableIdentifier);
            Positionals.Add(commandPositional);
            _currentContext = commandPositional;
            return this;
        }

        /// <summary>
        /// Add a positional parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter</param>
        public CommandConfig<T> Positional(string parameterName)
        {
            var commandPositional = ConfigGenerator.PositionalByName<T>(parameterName);
            Positionals.Add(commandPositional);
            _currentContext = commandPositional;
            return this;
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
        /// Specifies the simplest possible option type - there are no parameters, the option is simply present.
        /// However, some parsing conventions allow for a boolean to be specified allowing a false to be supplied.
        /// In order to support this, the option must accept a boolean and apply it appropriately.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionVariableIndicator">The expression that identifies the boolean that should be set in the command type.</param>
        /// <returns>The command config.</returns>
        public CommandConfig<T> Option(string optionName, Expression<Func<T, bool>> optionVariableIndicator)
        {
            var commandOption = ConfigGenerator.OptionFromExpression<T>(optionName, optionVariableIndicator, true);
            Options.Add(commandOption);
            _currentContext = commandOption;
            return this;
        }

        /// <summary>
        /// Specifies an option taking a single parameter. The property related to the option is identified using a Linq expression.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionVariableIndicator">The expression that identifies the member that should be set in the command type. The type of the member determines the data type of the option.</param>
        /// <returns>The command config.</returns>
        public CommandConfig<T> Option<TParam>(string optionName, Expression<Func<T, TParam>> optionVariableIndicator)
        {
            var commandOption = ConfigGenerator.OptionFromExpression<T>(optionName, optionVariableIndicator, false);
            Options.Add(commandOption);
            _currentContext = commandOption;
            return this;
        }

        /// <summary>
        /// Specifies an option taking a single parameter. The option is set using a caller supplied lambda expression.
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
        /// Specifies an option taking a single parameter. The property related to the option is identified automatically using the option name.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <returns>The command config.</returns>
        public CommandConfig<T> Option(string optionName)
        {
            var commandOption = ConfigGenerator.OptionByName<T>(optionName);
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

        internal override object Create(string commandName)
        {
            return _initialiser(commandName);
        }

        internal override bool Validate(object command, IList<string> messages)
        {
            if (_validator == null || command == null) return true;
            if (command is T) 
                return _validator(command as T, messages);

            messages.Add(String.Format("Internal error: Command type received was {0}, but {1} was expected.", command.GetType(), typeof (T)));
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
}