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
    /// This class generates Linq Expressions containing lambdas that assign properties of a command classes. 
    /// </summary>
    internal static class ParameterAssignmentGenerator<T> where T : class
    {
        public static object Generate(PropertyInfo prop, out Type[] parameterTypes, MemberInfo parent = null)
        {
            return Generate(prop, prop.PropertyType, out parameterTypes, parent);
        }

        public static object Generate(FieldInfo field, out Type[] parameterTypes, MemberInfo parent = null)
        {
            return Generate(field, field.FieldType, out parameterTypes, parent);
        }

        public static object Generate(MemberInfo memberInfo, Type memberType, out Type[] parameterTypes, MemberInfo parent = null)
        {
            var parameters = new List<ParameterExpression>();
            var item = Expression.Parameter(typeof(T), "command");
            parameters.Add(item);
            if (IsSimpleAssignment(memberType))
            {
                var value = Expression.Parameter(memberType, "value");
                parameters.Add(value);
                var expression = ConstructResultAssigment(memberInfo, memberType, item, value, parent);
                var genericMakeLambda = typeof (ParameterAssignmentGenerator<T>).GetMethod("MakeLambda1", BindingFlags.NonPublic | BindingFlags.Static);
                var makeLambda = genericMakeLambda.MakeGenericMethod(new[] {memberType});
                parameterTypes = new[] { memberType };
                return makeLambda.Invoke(null, new object[] {expression, parameters.ToArray()});
            }

            ValidateNestedType(memberType);

            var properties = memberType.GetProperties()
                .Select(p => new {Member = p as MemberInfo, Type = p.PropertyType, Position = p.GetCustomAttribute<PositionalAttribute>()});
            var fields = memberType.GetFields()
                .Select(f => new {Member = f as MemberInfo, Type = f.FieldType, Position = f.GetCustomAttribute<PositionalAttribute>()});

            var nestedParameters = properties
                .Concat(fields)
                .Select(m => new { m.Member, m.Type, Position = m.Position == null ? 0 : m.Position.Index })
                .OrderBy(m => m.Position);

            var nestedValues = nestedParameters
                .Select(n => new {Details = n, Value = Expression.Parameter(n.Type, n.Member.Name)})
                .ToList();
            parameters.AddRange(nestedValues.Select(n => n.Value));
            var newExpression = Expression.New(memberType);
            var inits = nestedValues.Select(n => Expression.Bind(n.Details.Member, n.Value));
            var memberInit = Expression.MemberInit(newExpression, inits);

            var genericMakeLambdaN = typeof(ParameterAssignmentGenerator<T>).GetMethod(string.Format("MakeLambda{0}", nestedValues.Count),
                BindingFlags.NonPublic | BindingFlags.Static);
            var makeLambdaN = genericMakeLambdaN.MakeGenericMethod(nestedValues.Select(p => p.Value.Type).ToArray());
            var body = ConstructResultAssigment(memberInfo, memberType, item, memberInit, parent);
            parameterTypes = nestedValues.Select(n => n.Details.Type).ToArray();
            return makeLambdaN.Invoke(null, new object[] { body, parameters.ToArray() });
        }

        private static Expression ConstructResultAssigment(MemberInfo member, Type memberType, ParameterExpression item, Expression value, MemberInfo parent)
        {
            Expression source;
            if (parent == null)
                source = item;
            else
                source = Expression.MakeMemberAccess(item, parent);

            return Expression.Assign(Expression.MakeMemberAccess(source, member), value);
        }

        private static bool IsSimpleAssignment(Type propertyType)
        {
            return Type.GetTypeCode(propertyType) != TypeCode.Object;
        }

        // ReSharper disable UnusedMember.Local

        private static Action<T, T1> MakeLambda1<T1>(Expression body, ParameterExpression[] parameters)
        {
            return Expression.Lambda<Action<T, T1>>(body, parameters).Compile();
        }

        private static Action<T, T1, T2> MakeLambda2<T1, T2>(Expression body, ParameterExpression[] parameters)
        {
            return Expression.Lambda<Action<T, T1, T2>>(body, parameters).Compile();
        }

        private static Action<T, T1, T2, T3> MakeLambda3<T1, T2, T3>(Expression body, ParameterExpression[] parameters)
        {
            return Expression.Lambda<Action<T, T1, T2, T3>>(body, parameters).Compile();
        }

        private static Action<T, T1, T2, T3, T4> MakeLambda4<T1, T2, T3, T4>(Expression body, ParameterExpression[] parameters)
        {
            return Expression.Lambda<Action<T, T1, T2, T3, T4>>(body, parameters).Compile();
        }

        // ReSharper restore UnusedMember.Local

        private static void ValidateNestedType(Type memberType)
        {
            if (MixedPropertiesAndFields(memberType))
                throw new NestedOptionTypeInvalid(string.Format("Mixed fields and properties are not allowed " 
                    + "in nested option types because parameter order cannot be deduced. Type: {0}",
                        memberType.Name), memberType);

            if (memberType.GetConstructor(new Type[]{}) == null)
                throw new NestedOptionTypeInvalid(string.Format("Nested option types must have a default constructor. Type: {0}",
                        memberType.Name), memberType);
        }

        private static bool MixedPropertiesAndFields(Type memberType)
        {
            var fields = memberType.GetFields();
            var properties = memberType.GetProperties();

            if (fields.All(f => f.GetCustomAttribute<PositionalAttribute>() != null) &&
                properties.All((f => f.GetCustomAttribute<PositionalAttribute>() != null)))
                return false;

            return fields.Length > 0 && properties.Length > 0;
        }
    }
}
