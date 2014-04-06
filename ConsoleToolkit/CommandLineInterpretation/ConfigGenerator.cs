using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public static class ConfigGenerator
    {
        /// <summary>
        /// Generate a <see cref="CommandPositional{T,TParameter}"/> using a parameter name
        /// string and the type of the command object. The parameter name will be matched to a property of
        /// the command type <see cref="T"/> and this will be used to generate a typed positional parameter.
        /// 
        /// An exact match between the parameter name and a property is preferred, but if there is no exact 
        /// match, a case insensitive match will be acceptable.
        /// </summary>
        /// <typeparam name="T">The command type. The parameter name must match one of its properties.</typeparam>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The instantiated positional paramter.</returns>
        public static BasePositional PositionalByName<T>(string parameterName) where T : class
        {
            var prop = MatchProperty(typeof (T), parameterName);
            var parameter = Expression.Parameter(typeof (T));
            var delegateType = typeof(Func<,>).MakeGenericType(new[] { typeof(T), prop.PropertyType });
            var accessor = Expression.Lambda(delegateType, Expression.MakeMemberAccess(parameter, prop), new[] { parameter });

            var method = typeof (ConfigGenerator).GetMethod("PositionalFromExpression");
            var genericMethod = method.MakeGenericMethod(new[] {typeof (T), prop.PropertyType});
            return genericMethod.Invoke(null, new object[] {parameterName, accessor}) as BasePositional;
        }

        /// <summary>
        /// Generate a <see cref="CommandPositional{T,TParameter}"/> using a linq expression to determine a field in the command object (<see cref="T"/>)
        /// that will receive the parameter value. The field indicated will be used to generate an <see cref="Action"/> that applies the argument 
        /// value to the parameter.
        /// </summary>
        /// <typeparam name="T">The command data type.</typeparam>
        /// <typeparam name="TParameter">The type of the field.</typeparam>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="positionalVariableIdentifier">The expression that identifies the field that should receive the parameter value.</param>
        /// <returns>The constructed <see cref="CommandPositional{T,TParameter}"/>.</returns>
        public static CommandPositional<T, TParameter> PositionalFromExpression<T, TParameter>(string parameterName, Expression<Func<T, TParameter>> positionalVariableIdentifier) where T : class
        {
            var positionalInitialiser = InitialiserFromExpression(positionalVariableIdentifier);
            return new CommandPositional<T, TParameter>(parameterName, positionalInitialiser);
        }

        private static Action<T, TParameter> InitialiserFromExpression<T, TParameter>(Expression<Func<T, TParameter>> positionalIdentifier)
        {
            var inputMemberExpression = positionalIdentifier.Body as MemberExpression;
            if (inputMemberExpression == null)
                throw new MemberReferenceExpected();

            var commandTypeInstance = Expression.Parameter(typeof (T));
            var convertedParameterValue = Expression.Parameter(typeof (TParameter));
            var memberAccess = Expression.MakeMemberAccess(commandTypeInstance, inputMemberExpression.Member);
            var expression = Expression.Assign(memberAccess, convertedParameterValue);
            var lambdaExpression = Expression.Lambda<Action<T, TParameter>>(expression,
                new[] {commandTypeInstance, convertedParameterValue});
            return lambdaExpression.Compile();
        }

        /// <summary>
        /// Generate a <see cref="CommandOption{TAction}"/> using a parameter name
        /// string and the type of the command object. The parameter name will be matched to a property of
        /// the command type <see cref="T"/> and this will be used to generate a typed option.
        /// 
        /// An exact match between the parameter name and a property is preferred, but if there is no exact 
        /// match, a case insensitive match will be acceptable.
        /// </summary>
        /// <typeparam name="T">The command type. The option name must match one of its properties.</typeparam>
        /// <param name="optionName">The name of the parameter.</param>
        /// <returns>The instantiated positional paramter.</returns>
        public static BaseOption OptionByName<T>(string optionName) where T : class
        {
            var prop = MatchProperty(typeof(T), optionName);
            var parameter = Expression.Parameter(typeof(T));
            var delegateType = typeof(Func<,>).MakeGenericType(new[] { typeof(T), prop.PropertyType });
            var accessor = Expression.Lambda(delegateType, Expression.MakeMemberAccess(parameter, prop), new[] { parameter });
            return OptionFromExpression<T>(optionName, accessor, prop.PropertyType == typeof(bool));
        }

        /// <summary>
        /// Match the parameter to a property by name, exact match preferred, or throw an exception.
        /// </summary>
        /// <param name="type">The command type that must contain the matching property.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The matching property.</returns>
        private static PropertyInfo MatchProperty(Type type, string parameterName)
        {
            var exactMatch = type.GetProperty(parameterName);
            if (exactMatch != null)
                return exactMatch;

            var caseInsensitiveMatch = type.GetProperties().FirstOrDefault(p => String.Compare(parameterName, p.Name, true) == 0);
            if (caseInsensitiveMatch != null)
                return caseInsensitiveMatch;

            throw new NoMatchingPropertyFoundException(parameterName, type);
        }

        public static BaseOption OptionFromExpression<TCommand>(string optionName, 
            LambdaExpression optionVariableIndicator, 
            bool isBooleanOption) where TCommand : class
        {
            var setter = SetterBuilder.Build<TCommand>(optionVariableIndicator.Body);

            var optionGenericType = typeof(CommandOption<>);
            var optionType = optionGenericType.MakeGenericType(new[] {setter.GetType()});

            var option = Activator.CreateInstance(optionType, new[] {optionName, setter}) as BaseOption;
            if (option != null)
                option.IsBoolean = isBooleanOption;

            return option;
        }
    }
}