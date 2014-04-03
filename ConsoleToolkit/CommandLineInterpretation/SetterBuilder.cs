using System;
using System.Linq.Expressions;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal static class SetterBuilder
    {
        public static object Build<T>(Expression body)
        {
            var inputMemberExpression = body as MemberExpression;
            if (inputMemberExpression == null)
                throw new MemberReferenceExpected();

            var commandTypeInstance = Expression.Parameter(typeof(T));
            var convertedParameterValue = Expression.Parameter(inputMemberExpression.Type);
            var memberAccess = Expression.MakeMemberAccess(commandTypeInstance, inputMemberExpression.Member);
            var expression = Expression.Assign(memberAccess, convertedParameterValue);
            var delegateType =
                typeof (Action<,>).MakeGenericType(new[] {commandTypeInstance.Type, convertedParameterValue.Type});
            var lambdaExpression = Expression.Lambda(delegateType, expression, new []{commandTypeInstance, convertedParameterValue});
            return lambdaExpression.Compile();
        }
    }
}