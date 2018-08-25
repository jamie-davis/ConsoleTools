using System;

namespace ApprovalTests.Reporters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UseReporterAttribute : Attribute
    {
        public Type ReporterType { get; }

        public UseReporterAttribute(Type reporterType)
        {
            ReporterType = reporterType;
        }
    }
}