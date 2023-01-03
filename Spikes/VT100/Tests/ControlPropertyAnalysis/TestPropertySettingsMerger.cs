using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.ControlPropertyAnalysis;
using Xunit;

namespace VT100.Tests.ControlPropertyAnalysis
{
    public class TestPropertySettingsMerger
    {
        [Fact]
        public void SettingsAreMerged()
        {
            //Arrange
            var currentSettings = new List<PropertySetting>
            {
                new PropertySetting<string>("Prop1", "value1"),
                new PropertySetting<string>("Prop2", "value2"),
                new PropertySetting<string>("Prop3", "value3"),
                new PropertySetting<string>("Prop4", "value4"),
                new PropertySetting<string>("Prop5", "value5"),
            };

            var mergeSettings = new List<PropertySetting>
            {
                new PropertySetting<string>("Prop3", "replaced3"),
                new PropertySetting<string>("Prop5", "replaced5"),
                new PropertySetting<string>("Prop6", "value6"),
            };
            
            //Act
            var result = PropertySettingsMerger.Merge(currentSettings, new ReadOnlyCollection<PropertySetting>(mergeSettings));
            
            //Assert
            var output = new Output();
            output.WrapLine("Original set:");
            output.WriteLine();
            output.FormatTable(currentSettings.Select(p => new { p.Property, Value = p.GetValue()}));
            output.WriteLine();
            output.WriteLine();
            output.WrapLine("Override set:");
            output.WriteLine();
            output.FormatTable(mergeSettings.Select(p => new { p.Property, Value = p.GetValue()}));
            output.WriteLine();
            output.WriteLine();
            output.WrapLine("Merged set:");
            output.WriteLine();
            output.FormatTable(result.Select(p => new { p.Property, Value = p.GetValue()}));
            output.Report.Verify();
        }
    }
}