using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// The configuration information for the global options of a command driven application.
    /// </summary>
    public sealed class GlobalOptionsConfig<T> : BaseGlobalOptionsConfig, IOptionContainer<T, GlobalOptionsConfig<T>> where T : class
    {
        private IContext _currentContext;

        /// <summary>
        /// Specifies the simplest possible option type - there are no parameters, the option is simply present.
        /// However, some parsing conventions allow for a boolean to be specified allowing a false to be supplied.
        /// In order to support this, the option must accept a boolean and apply it appropriately.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionInitialiser">The lambda that applies the option to the command parameters type. Note that this must accept a boolean.</param>
        /// <returns>The command config.</returns>
        public GlobalOptionsConfig<T> Option(string optionName, Action<T, bool> optionInitialiser)
        {
            var commandOption = new CommandOption<Action<T, bool>>(optionName, optionInitialiser) { IsBoolean = true };
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
        public GlobalOptionsConfig<T> Option(string optionName, Expression<Func<T, bool>> optionVariableIndicator)
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
        public GlobalOptionsConfig<T> Option<TParam>(string optionName, Expression<Func<T, TParam>> optionVariableIndicator)
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
        public GlobalOptionsConfig<T> Option<T1>(string optionName, Action<T, T1> optionInitialiser)
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
        public GlobalOptionsConfig<T> Option(string optionName)
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
        public GlobalOptionsConfig<T> Option<T1, T2>(string optionName, Action<T, T1, T2> optionInitialiser)
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
        public GlobalOptionsConfig<T> Option<T1, T2, T3>(string optionName, Action<T, T1, T2, T3> optionInitialiser)
        {
            var commandOption = new CommandOption<Action<T, T1, T2, T3>>(optionName, optionInitialiser);
            Options.Add(commandOption);
            _currentContext = commandOption;
            return this;
        }

        /// <summary>
        /// Use this method to provide descriptive text. The description is context sensitive and will be applied to the command, 
        /// option or parameter that is currently being configured. Therefore, you must specify the appropriate description before 
        /// configuring more detail.
        /// </summary>
        /// <param name="text">The descriptive test.</param>
        /// <returns>The command config.</returns>
        public GlobalOptionsConfig<T> Description(string text)
        {
            if (_currentContext != null)
                _currentContext.Description = text;
            return this;
        }
    }

    public class BaseGlobalOptionsConfig
    {
        private List<BaseOption> _baseOptions = new List<BaseOption>();

        public List<BaseOption> Options => _baseOptions;
    }
}