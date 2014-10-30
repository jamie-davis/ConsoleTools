using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ConsoleToolkit.Exceptions;

namespace ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions
{
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
}