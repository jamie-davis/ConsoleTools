using System;
using System.Linq.Expressions;
using ConsoleToolkit.Exceptions;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal static class SetterBuilder
    {
        public static object Build(Type target, Expression body)
        {
            if (!(body is MemberExpression inputMemberExpression))
                throw new MemberReferenceExpected();

            var staticMember = inputMemberExpression.Member.IsStatic();
            var commandTypeInstance = Expression.Parameter(target);
            var convertedParameterValue = Expression.Parameter(inputMemberExpression.Type);
            var parent = staticMember ? null : commandTypeInstance;
            var memberAccess = Expression.MakeMemberAccess(parent, inputMemberExpression.Member);
            var expression = Expression.Assign(memberAccess, convertedParameterValue);
            Type delegateType;
            ParameterExpression[] parameters;

            if (staticMember)
            {
                delegateType = typeof(Action<>).MakeGenericType(convertedParameterValue.Type);
                parameters = new[] { convertedParameterValue };
            }
            else
            {
                delegateType = typeof(Action<,>).MakeGenericType(commandTypeInstance.Type, convertedParameterValue.Type);
                parameters = new []{commandTypeInstance, convertedParameterValue};
            }

            var lambdaExpression = Expression.Lambda(delegateType, expression, parameters);
            return lambdaExpression.Compile();
        }
    }
}
