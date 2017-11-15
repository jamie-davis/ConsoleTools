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
    public interface IOptionContainer<TCommandConfigType>
    {
        /// <summary>
        /// A list of extracted options.
        /// </summary>
        List<BaseOption> Options { get; }

        /// <summary>
        /// Use this method to provide descriptive text. The description is context sensitive and will be applied to the option
        /// that is currently being configured.
        /// </summary>
        /// <param name="text">The descriptive test.</param>
        /// <returns>The config.</returns>
        TCommandConfigType Description(string text);
    }
}