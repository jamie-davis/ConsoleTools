using System.Linq.Expressions;
using System.Reflection;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO.ReportDefinitions
{
    internal static class ColumnNameExtractor
    {
        public static string FromExpression(Expression valueExpression)
        {
            if (valueExpression is LambdaExpression)
            {
                return FromExpression((valueExpression as LambdaExpression).Body);
            }

            string memberName = null;
            var memberExpression = valueExpression as MemberExpression;
            if (memberExpression != null)
            {
                var property = memberExpression.Member as PropertyInfo;
                if (property != null)
                    memberName = property.Name;
                else
                {
                    var field = memberExpression.Member as FieldInfo;
                    if (field != null)
                    memberName = field.Name;}
            }
                
            if (memberName == null) return "exp";

            return PropertyNameConverter.ToHeading(memberName);
        }
    }
}