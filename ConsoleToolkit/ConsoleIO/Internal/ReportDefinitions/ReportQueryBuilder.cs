using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions
{
    internal static class ReportQueryBuilder
    {
        private const string RowFieldName = "row";
        private static RunTimeTypeBuilder _runtimeTypeBuilder;

        static ReportQueryBuilder()
        {
            _runtimeTypeBuilder = new RunTimeTypeBuilder("reportqueryassembly");
        }

        public static IEnumerable Build<T>(IEnumerable<T> items, IEnumerable<Expression> reportParameters, out Type rowType, out Func<object, T> originalRowGetter)
        {
            var expressions = reportParameters.Select(MakeQueryExpression).ToList();
            var expressionId = 0;
            var builderProperties = expressions
                .Select(e => new RuntimeTypeBuilderProperty(string.Format("exp{0}", ++expressionId), e.ReturnType))
                .Concat(new[] {new RuntimeTypeBuilderProperty(RowFieldName, typeof (T))});
            
            rowType = _runtimeTypeBuilder.MakeRuntimeType(builderProperties);

            var genericMethod = typeof (ReportQueryRowFunctionBuilder).GetMethod("MakeQueryFunction");
            var method = genericMethod.MakeGenericMethod(rowType);

            var func = MethodInvoker.Invoke(method, null, new object[] {expressions}) as Func<object, object>;

            var runQuery = typeof (ReportQueryBuilder).GetMethod("RunQuery", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(rowType);

            originalRowGetter = MakeOriginalRowGetter<T>(rowType);
            return MethodInvoker.Invoke(runQuery, null, new object[] {items.Cast<object>(), func}) as IEnumerable;
        }

        // ReSharper disable once UnusedMember.Local
        // This method is called by code generated in Build<T>.
        private static IEnumerable RunQuery<TOutput>(IEnumerable<object> items, Func<object, TOutput> func)
        {
            return items.Select(func);
        }

        private static Func<object, T> MakeOriginalRowGetter<T>(Type rowType)
        {
            var param = Expression.Parameter(typeof (object));
            var body = Expression.MakeMemberAccess(Expression.Convert(param, rowType),
                rowType.GetProperty(RowFieldName));
            return Expression.Lambda<Func<object, T>>(body, new [] {param}).Compile();
        }

        private static ReportQueryExpression MakeQueryExpression(Expression arg)
        {
            var lamda = arg as LambdaExpression;
            if (lamda == null) 
                throw new BadExpressionType(arg);
            return new ReportQueryExpression
                       {
                           Expression = lamda.Body,
                           ReturnType = lamda.ReturnType,
                           ParameterVariable = lamda.Parameters.First()
                       };
        }

    }

    internal class ReportQueryExpression
    {
        public Expression Expression { get; set; }
        public Type ReturnType { get; set; }
        public ParameterExpression ParameterVariable { get; set; }
    }

    internal class BadExpressionType : Exception
    {
        public BadExpressionType(Expression expression) 
            : base(string.Format("The expression \"{0}\" is not acceptable in a report.", expression))
        {
        }
    }

    internal class MixedInputTypesInQueryExpressions : Exception
    {
    }

    internal class ResultTypeCannotAcceptQuery : Exception
    {
    }
}