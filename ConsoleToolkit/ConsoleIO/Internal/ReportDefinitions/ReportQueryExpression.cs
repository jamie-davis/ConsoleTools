using System;
using System.Linq.Expressions;

namespace ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions
{
    internal class ReportQueryExpression
    {
        public Expression Expression { get; set; }
        public Type ReturnType { get; set; }
        public ParameterExpression ParameterVariable { get; set; }
    }
}