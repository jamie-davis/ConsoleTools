using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// This interface defines a type that can hold command options. This abstraction allows option parsing to be shared between different options sources, such as command definitions and global option definitions.
    /// </summary>
    /// <typeparam name="T">The type that the extracted options will target.</typeparam>
    /// <typeparam name="TCommandConfigType">The (fluent) command config that implements this interface. (The option methods are part of a fluent interface and must return the correct container type.)</typeparam>
    public interface IOptionContainer<T, TCommandConfigType> where T : class
    {
        /// <summary>
        /// A list of extracted options.
        /// </summary>
        List<BaseOption> Options { get; }

        /// <summary>
        /// Specifies the simplest possible option type - there are no parameters, the option is simply present.
        /// However, some parsing conventions allow for a boolean to be specified allowing a false to be supplied.
        /// In order to support this, the option must accept a boolean and apply it appropriately.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionInitialiser">The lambda that applies the option to the command parameters type. Note that this must accept a boolean.</param>
        /// <returns>The command config.</returns>
        TCommandConfigType Option(string optionName, Action<T, bool> optionInitialiser);

        /// <summary>
        /// Specifies the simplest possible option type - there are no parameters, the option is simply present.
        /// However, some parsing conventions allow for a boolean to be specified allowing a false to be supplied.
        /// In order to support this, the option must accept a boolean and apply it appropriately.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionVariableIndicator">The expression that identifies the boolean that should be set in the command type.</param>
        /// <returns>The command config.</returns>
        TCommandConfigType Option(string optionName, Expression<Func<T, bool>> optionVariableIndicator);

        /// <summary>
        /// Specifies an option taking a single parameter. The property related to the option is identified using a Linq expression.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionVariableIndicator">The expression that identifies the member that should be set in the command type. The type of the member determines the data type of the option.</param>
        /// <returns>The command config.</returns>
        TCommandConfigType Option<TParam>(string optionName, Expression<Func<T, TParam>> optionVariableIndicator);

        /// <summary>
        /// Specifies an option taking a single parameter. The option is set using a caller supplied lambda expression.
        /// </summary>
        /// <typeparam name="T1">The parameter.</typeparam>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionInitialiser">The action that will be invoked when the option's parameters have been converted.</param>
        /// <returns>The command config.</returns>
        TCommandConfigType Option<T1>(string optionName, Action<T, T1> optionInitialiser);

        /// <summary>
        /// Specifies an option taking a single parameter. The property related to the option is identified automatically using the option name.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <returns>The command config.</returns>
        TCommandConfigType Option(string optionName);

        /// <summary>
        /// Specifies an option taking a two parameters.
        /// </summary>
        /// <typeparam name="T1">The first parameter.</typeparam>
        /// <typeparam name="T2">The second parameter.</typeparam>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionInitialiser">The action that will be invoked when the option's parameters have been converted.</param>
        /// <returns>The command config.</returns>
        TCommandConfigType Option<T1, T2>(string optionName, Action<T, T1, T2> optionInitialiser);

        /// <summary>
        /// Specifies an option taking a two parameters.
        /// </summary>
        /// <typeparam name="T1">The first parameter.</typeparam>
        /// <typeparam name="T2">The second parameter.</typeparam>
        /// <typeparam name="T3">The third parameter.</typeparam>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="optionInitialiser">The action that will be invoked when the option's parameters have been converted.</param>
        /// <returns>The command config.</returns>
        TCommandConfigType Option<T1, T2, T3>(string optionName, Action<T, T1, T2, T3> optionInitialiser);

        /// <summary>
        /// Use this method to provide descriptive text. The description is context sensitive and will be applied to the option
        /// that is currently being configured.
        /// </summary>
        /// <param name="text">The descriptive test.</param>
        /// <returns>The config.</returns>
        TCommandConfigType Description(string text);
    }
}