using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FuncDelegate = System.Func<object, System.Collections.Generic.IEnumerable<object>, object>;

namespace ConsoleToolkit.Utilities
{
    /// <summary>
    /// This class is able to generate a function to call the methods indicated by supplied MethodInfo instances.
    /// The generated functions are retained so that if a method has been invoked before, the previously generated 
    /// function can be reused.<para/>
    /// 
    /// The purpose of the class is to allow exceptions to retain their call stack when they are thrown from methods 
    /// invoked via reflection. If <see cref="MethodInfo"/>'s Invoke(...) overloads are used, exceptions are wrapped in
    /// a <see cref="TargetInvocationException"/>, and throwing the <see cref="Exception.InnerException"/> rewrites
    /// the call stack. Under .NET 4.5, exceptions can be rethrown with their original call stack, but there is no
    /// way to do that under .NET 4.0. However, the Linq based solution works in both versions.
    /// </summary>
    internal static class MethodInvoker
    {
        private static object _lock = new object();

        private static readonly Dictionary<MethodInfo, FuncDelegate> MethodFunctions = new Dictionary<MethodInfo, FuncDelegate>();

        private static readonly MethodInfo GetEnumeratorMethod = typeof(IEnumerable<object>).GetMethod("GetEnumerator");
        private static readonly MethodInfo MoveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
        private static readonly PropertyInfo GetCurrent = typeof(IEnumerator<object>).GetProperty("Current");

        internal static object Invoke(MethodInfo method, object handler, params object[] parameters)
        {
            lock (_lock)
            {
                FuncDelegate func;
                if (!MethodFunctions.TryGetValue(method, out func))
                {
                    func = Generate(method);
                    MethodFunctions[method] = func;
                }

                return func(handler, parameters ?? new object[0]);
            }
        }

        private static FuncDelegate Generate(MethodInfo method)
        {
            var expressions = new List<Expression>();

            //make function parameters
            var handlerVar = Expression.Variable(typeof(object), "obj");
            var parametersVar = Expression.Parameter(typeof (IEnumerable<object>), "params");

            //make variables for each of the method parameters
            var methodParamVars = method.GetParameters()
                .Select(p => Expression.Variable(p.ParameterType, p.Name)).ToList();

            //copy the parameters from the supplied array of objects into the parameter variables
            var iterator = Expression.Variable(typeof (IEnumerator<object>), "enumerator");
            expressions.Add(Expression.Assign(iterator, Expression.Call(parametersVar, GetEnumeratorMethod)));
            foreach (var methodParamVar in methodParamVars)
            {
                expressions.Add(Expression.Call(iterator, MoveNextMethod));
                expressions.Add(Expression.Assign(methodParamVar, Expression.Convert(Expression.MakeMemberAccess(iterator, GetCurrent), methodParamVar.Type)));
            }

            //Make the call expression
            Expression call;
            if (method.IsStatic)
                call = Expression.Call(null, method, methodParamVars);
            else
                call = Expression.Call(Expression.Convert(handlerVar, method.DeclaringType), method,
                                                methodParamVars);

            //void methods need to conform to the FuncDelegate return value, so return null.
            if (method.ReturnType == typeof (void))
            {
                expressions.Add(call);
                expressions.Add(Expression.Constant(null));
            }
            else
                expressions.Add(Expression.Convert(call, typeof(object)));

            //construct the function
            var parameters = methodParamVars.Concat(new [] {iterator});
            var block = Expression.Block(typeof(object), parameters, expressions);
            return Expression.Lambda<FuncDelegate>(block, new[] {handlerVar, parametersVar}).Compile();
        }
    }

}
