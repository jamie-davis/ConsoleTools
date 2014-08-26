using System;
using System.Linq;
using System.Linq.Expressions;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// Generate a lambda that constructs an instance of a command class.
    /// </summary>
    internal static class CommandConstructionLambdaGenerator<T> where T : new() 
    {
        /// <summary>
        /// Generate a function to create an instance of the specified type, in the context of command
        /// types that may contain option sets.
        /// <seealso cref="OptionSetAttribute"/>
        /// </summary>
        /// <returns>A <see cref="Func{T}"/> that creates an instance of the generic type.</returns>
        internal static Func<T> Generate()
        {
            var optionSets = typeof (T).GetProperties()
                .Where(p => p.GetCustomAttribute<OptionSetAttribute>() != null)
                .ToList();

            if (!optionSets.Any())
                return () => new T();

            var initialisers = optionSets.Select(o => Expression.Bind(o, Expression.New(o.PropertyType)));
            var createExpression = Expression.New(typeof (T));
            var memberInit = Expression.MemberInit(createExpression, initialisers);
            return Expression.Lambda<Func<T>>(memberInit).Compile();
        }
    }
}