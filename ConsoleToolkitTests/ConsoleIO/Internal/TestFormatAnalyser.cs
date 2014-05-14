using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestFormatAnalyser
    {
        class TestType
        {
            public int IntCol { get; set; }
            public string StringCol { get; set; }
            public DateTime DateTimeCol { get; set; }
            public double DoubleCol { get; set; }
            public float FloatCol { get; set; }
            public decimal DecimalCol { get; set; }
// ReSharper disable once InconsistentNaming
            //This property name must not be split on upper case characters
            public int ABC { get; set; }
        }

        [Test]
        public void NoColumnDefinitionReturnsAFullSetOfColumnFormats()
        {
            var propFormats = FormatAnalyser.Analyse(typeof (TestType), null);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Test]
        public void ExtraColumnDefinitionsAreIgnored()
        {
            var cols = typeof (TestType).GetProperties()
                .Select(p => new ColumnFormat("Expected", p.PropertyType))
                .Concat(Enumerable.Range(0, 5).Select(i => new ColumnFormat("Extra")));
            var propFormats = FormatAnalyser.Analyse(typeof(TestType), cols);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Test]
        public void DefaultsAreProvidedForMissingFormats()
        {
            var cols = typeof (TestType).GetProperties().Take(2)
                .Select(p => new ColumnFormat("Included", p.PropertyType));
            var propFormats = FormatAnalyser.Analyse(typeof(TestType), cols);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Test]
        public void DefaultsAreProvidedForSkippedFormats()
        {
            var cols = typeof (TestType).GetProperties()
                .Select(p => p.Name == "StringCol" ? new ColumnFormat("My String", p.PropertyType) : null);
            var propFormats = FormatAnalyser.Analyse(typeof(TestType), cols);
            Approvals.Verify(ReportFormats(propFormats));
        }

        private string ReportFormats(List<PropertyColumnFormat> propFormats)
        {
            var sb = new StringBuilder();

            foreach (var propertyColumnFormat in propFormats)
            {
                sb.AppendFormat("{0,-11} = {1}", propertyColumnFormat.Property.Name, propertyColumnFormat.Format);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}