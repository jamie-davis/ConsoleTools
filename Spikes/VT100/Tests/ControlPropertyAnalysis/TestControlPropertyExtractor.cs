using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.Attributes;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen;
using Xunit;

namespace VT100.Tests.ControlPropertyAnalysis
{
    /// <summary>
    /// This class analyses a type or property definition to extract any control property meta data defined on it. 
    /// </summary>
    public class TestControlPropertyExtractor
    {
        #region Types for test

        [Background(VtColour.Green)]
        [Foreground(VtColour.White)]
        [InputForeground(VtColour.BrightYellow)]
        class AttributedClass
        {
            [InputBackground(VtColour.Black)]
            [InputForeground(VtColour.White)]
            public string Input { get; set; }
        }
        
        #endregion

        
        [Fact]
        public void PropertyAttributesAreIdentified()
        {
            //Assert
            var props = ControlPropertyExtractor.PropertyAttributes.Select(p => new { PropertyAttribute = p.Name });
            var output = new Output();
            output.FormatTable(props);
            output.Report.Verify();
        }

        [Fact]
        void AttributesAreExtractedFromType()
        {
            //Act
            var props = ControlPropertyExtractor.Extract(typeof(AttributedClass));
            
            //Assert
            var output = new Output();
            output.FormatTable(props.Select(p => new { p.Property, Value = p.GetValue(), ValueType = p.GetValue()?.GetType().Name }));
            output.Report.Verify();
        }

        [Fact]
        void AttributesAreExtractedFromProperty()
        {
            //Act
            var props = ControlPropertyExtractor.Extract(typeof(AttributedClass).GetProperty(nameof(AttributedClass.Input)));
            
            //Assert
            var output = new Output();
            output.FormatTable(props.Select(p => new { p.Property, Value = p.GetValue(), ValueType = p.GetValue()?.GetType().Name }));
            output.Report.Verify();
        }
    }
}