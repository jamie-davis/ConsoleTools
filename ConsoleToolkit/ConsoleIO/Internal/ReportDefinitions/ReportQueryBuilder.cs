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

    /// <summary>
    /// Builds a function that will convert a consistent input type which is not directly specified (i.e. only the expressions contain it), to the 
    /// provided output type using a set of expressions that extract values from the input type. <para/>
    /// 
    /// <remarks>
    /// An important assumption made by this process is that the output type will provide sufficient properties that match the types returned by the expressions.<para/>
    /// 
    /// Each expression will be tied to the "next" matching property by type alone. For example:<para/>
    /// <code>
    /// class ResultType
    /// {
    ///     public double D1 {get;set;}
    ///     public double D2 {get;set;}
    ///     public int    I1 {get;set;}
    ///     public double D3 {get;set;}
    /// }
    /// </code>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Expression Type</term>
    ///         <description>Expression</description>
    ///     </listheader> 
    ///     <item><term>Int</term><description>a.Age</description></item>
    ///     <item><term>Double</term><description>a.HeightMetres</description></item>
    ///     <item><term>Double</term><description>a.WeightKg</description></item>
    ///     <item><term>Double</term><description>a.WeightKg/a.HeightMetres</description></item>
    /// </list>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Result Property</term>
    ///         <description>Expression</description>
    ///     </listheader> 
    ///     <item><term>D1</term><description>a.HeightMetres</description></item>
    ///     <item><term>D2</term><description>a.WeightKg</description></item>
    ///     <item><term>I1</term><description>a.Age</description></item>
    ///     <item><term>D3</term><description>a.WeightKg/a.HeightMetres</description></item>
    /// </list>
    /// This is important, because callers must ensure that the result type has enough properties of each required type
    /// to receive all of the expression results, and that the order of the properties in the result type matches the
    /// order of the expressions within each distinct type.
    /// </remarks>
    /// </summary>
    internal static class ReportQueryRowFunctionBuilder
    {
        public static Func<object, T> MakeQueryFunction<T>(IEnumerable<ReportQueryExpression> expressions)
        {
            var outputType = typeof (T);
            Type inputType = null;
            var inputObject = Expression.Parameter(typeof (object), "itemObject");
            Expression convertedItem = null;
            var remainingProps = outputType.GetProperties().ToList();
            var resultList = new List<MemberAssignment>();
            foreach (var queryExpression in expressions)
            {
                var type = queryExpression.Expression.Type;
                var prop = remainingProps.FirstOrDefault(p => p.PropertyType == type);
                if (prop == null)
                    throw new ResultTypeCannotAcceptQuery();

                remainingProps.Remove(prop);

                if (inputType == null)
                {
                    inputType = queryExpression.ParameterVariable.Type;
                    convertedItem = Expression.Convert(inputObject, inputType);
                }
                else if (inputType != queryExpression.ParameterVariable.Type)
                    throw new MixedInputTypesInQueryExpressions();

                var setter = Expression.Block(type, new[] {queryExpression.ParameterVariable},
                                              Expression.Assign(queryExpression.ParameterVariable, convertedItem),
                                              queryExpression.Expression);
                resultList.Add(Expression.Bind(prop, setter));
            }

            var rowProp = remainingProps.FirstOrDefault(p => p.Name == "row");
            if (rowProp != null)
                resultList.Add(Expression.Bind(rowProp, Expression.Convert(inputObject, rowProp.PropertyType)));

            return Expression.Lambda<Func<object, T>>(Expression.MemberInit(Expression.New(outputType), resultList), false, inputObject).Compile();
        }
    }

    internal class MixedInputTypesInQueryExpressions : Exception
    {
    }

    internal class ResultTypeCannotAcceptQuery : Exception
    {
    }
}