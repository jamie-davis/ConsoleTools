using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        /// types that may contain option sets and <see cref="ICollection{T}"/> derived lists.
        /// <seealso cref="OptionSetAttribute"/>
        /// </summary>
        /// <returns>A <see cref="Func{T}"/> that creates an instance of the generic type.</returns>
        internal static Func<T> Generate()
        {
            var optionSets = typeof (T).GetProperties()
                .Where(p => p.GetCustomAttribute<OptionSetAttribute>() != null)
                .ToList();

            var lists = typeof (T).GetProperties()
                .Where(IsConstructableList)
                .ToList();

            if (!optionSets.Any() && !lists.Any())
                return () => new T();

            var optionSetInitialisers = optionSets.Select(o => Expression.Bind(o, Expression.New(o.PropertyType)));
            var listInitialisers = lists.Select(o => Expression.Bind(o, Expression.New(o.PropertyType)));
            var createExpression = Expression.New(typeof (T));
            var memberInit = Expression.MemberInit(createExpression, optionSetInitialisers.Concat(listInitialisers));
            return Expression.Lambda<Func<T>>(memberInit).Compile();
        }

        private static bool IsConstructableList(PropertyInfo property)
        {
            if (property.GetCustomAttribute<PositionalAttribute>() == null 
                && property.GetCustomAttribute<OptionAttribute>() == null)
                return false;

            if (!property.CanWrite)
                return false;

            if (!CollectionTypeAnalyser.IsCollectionType(property.PropertyType))
                return false;

            return property.PropertyType.GetConstructor(new Type[] {}) != null;
        }
    }
}