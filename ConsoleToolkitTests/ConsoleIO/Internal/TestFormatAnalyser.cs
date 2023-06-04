using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
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

        class TestTypeWithRenderable
        {
            public int IntCol { get; set; }
            public string StringCol { get; set; }
            public RecordingConsoleAdapter RenderCol { get; set; }
        }

        [Fact]
        public void NoColumnDefinitionReturnsAFullSetOfColumnFormats()
        {
            var propFormats = FormatAnalyser.Analyse(typeof (TestType), null, true);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Fact]
        public void ExtraColumnDefinitionsAreIgnored()
        {
            var cols = typeof (TestType).GetProperties()
                .Select(p => new ColumnFormat("Expected", p.PropertyType))
                .Concat(Enumerable.Range(0, 5).Select(i => new ColumnFormat("Extra")));
            var propFormats = FormatAnalyser.Analyse(typeof(TestType), cols, true);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Fact]
        public void ColumnsWithNoDefinitionAreExtracted()
        {
            var intColProp = typeof (TestType).GetProperty("IntCol");
            var cols = new [] {new ColumnFormat("Defined Col", intColProp.PropertyType)};
            var propFormats = FormatAnalyser.Analyse(typeof(TestType), cols, true);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Fact]
        public void ColumnsWithNoDefinitionAreOmittedWhenIncludeAllColumnsIsFalse()
        {
            var intColProp = typeof (TestType).GetProperty("IntCol");
            var cols = new [] {new ColumnFormat("Defined Col", intColProp.PropertyType)};
            var propFormats = FormatAnalyser.Analyse(typeof(TestType), cols, false);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Fact]
        public void DefaultsAreProvidedForMissingFormats()
        {
            var cols = typeof (TestType).GetProperties().Take(2)
                .Select(p => new ColumnFormat("Included", p.PropertyType));
            var propFormats = FormatAnalyser.Analyse(typeof(TestType), cols, true);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Fact]
        public void DefaultsAreProvidedForSkippedFormats()
        {
            var cols = typeof (TestType).GetProperties()
                .Select(p => p.Name == "StringCol" ? new ColumnFormat("My String", p.PropertyType) : null);
            var propFormats = FormatAnalyser.Analyse(typeof(TestType), cols, true);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Fact]
        public void DefaultRenderColumnIsGenerated()
        {
            var cols = typeof (TestType).GetProperties()
                .Select(p => p.Name == "StringCol" ? new ColumnFormat("My String", p.PropertyType) : null);
            var propFormats = FormatAnalyser.Analyse(typeof(TestTypeWithRenderable), cols, true);
            Approvals.Verify(ReportFormats(propFormats));
        }

        [Fact]
        public void PrimitivesProduceASingleColumn()
        {
            var propFormats = FormatAnalyser.Analyse(typeof(int), null, true);
            Approvals.Verify(ReportFormats(propFormats));
        }

        private string ReportFormats(List<PropertyColumnFormat> propFormats)
        {
            var sb = new StringBuilder();

            foreach (var propertyColumnFormat in propFormats)
            {
                sb.AppendFormat("{0,-11} = {1}", propertyColumnFormat.Property ==null ? "<null>" : propertyColumnFormat.Property.Name, propertyColumnFormat.Format);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}