using System.Linq;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen;
using Xunit;

namespace VT100.Tests.ControlPropertyAnalysis
{
    public class TestControlPropertySetter
    {
        [Fact]
        public void PropertiesAreExtracted()
        {
            //Arrange
            var props = new []
            {
                new PropertySetting<VtColour>("Background", VtColour.Black),
                new PropertySetting<VtColour>("Foreground", VtColour.White),
            };
            var propClass = new ScreenProps();

            //Act
            ControlPropertySetter.Set(propClass, props);

            //Assert
            var output = new Output();
            
            output.WrapLine("Property set:");
            output.WriteLine();
            output.FormatTable(props.Select(p => new { p.Property, p.Value }));
            output.WriteLine();
            output.WriteLine();
            output.WrapLine("Extracted:");
            output.WriteLine();
            output.FormatTable(new [] { propClass });
            output.Report.Verify();
        }
    }
}