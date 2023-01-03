using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
using VT100.Tests.Fakes;
using VT100.Utilities.ReadConsole;
using Xunit;
// ReSharper disable UnusedMember.Local

namespace VT100.Tests.ControlPropertyAnalysis
{
    public class TestControlSettingsUpdater
    {
        class SettingsType
        {
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
            public string Prop3 { get; set; }
        }

        class SettingsControl : BaseFakeControl, IFormattedLayoutControl<SettingsType>
        {
            private SettingsType _format;

            #region Implementation of IFormattedLayoutControl<SettingsType>

            public SettingsType Format
            {
                get => _format;
                set => _format = value;
            }

            #endregion
        }
        
        [Fact]
        public void ControlWithoutSettingsIsAccepted()
        {
            //Arrange
            var control = new BaseFakeControl();
            var mainSettings = new List<PropertySetting>();
            var controlSettings = new List<PropertySetting>();

            //Act/Assert
            ControlSettingsUpdater.Update(control, mainSettings, new ReadOnlyCollection<PropertySetting>(controlSettings));
            //there should be no exceptions
        }
        
        [Fact]
        public void ControlSettingsAreUpdated()
        {
            //Arrange
            var control = new SettingsControl();
            var currentSettings = new List<PropertySetting>
            {
                new PropertySetting<string>("Prop1", "value1"),
                new PropertySetting<string>("Prop2", "value2"),
                new PropertySetting<string>("Prop3", "value3"),
                new PropertySetting<string>("Prop4", "value4"),
                new PropertySetting<string>("Prop5", "value5"),
            };

            var controlSettings = new List<PropertySetting>
            {
                new PropertySetting<string>("Prop3", "replaced3"),
                new PropertySetting<string>("Prop5", "replaced5"),
                new PropertySetting<string>("Prop6", "value6"),
            };

            //Act
            ControlSettingsUpdater.Update(control, currentSettings, new ReadOnlyCollection<PropertySetting>(controlSettings));
            
            //Assert
            var output = new Output();
            output.WrapLine("Original settings:");
            output.WriteLine();
            output.FormatTable(currentSettings.Select(p => new { p.Property, Value = p.GetValue()}));
            output.WriteLine();
            output.WriteLine();
            output.WrapLine("Override set:");
            output.WriteLine();
            output.FormatTable(controlSettings.Select(p => new { p.Property, Value = p.GetValue()}));
            output.WriteLine();
            output.WriteLine();
            output.WrapLine("Settings from control:");
            output.WriteLine();
            output.FormatTable(new [] {control.Format});
            output.Report.Verify();
        }
    }
}