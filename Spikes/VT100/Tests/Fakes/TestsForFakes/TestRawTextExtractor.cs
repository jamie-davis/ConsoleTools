using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.FullScreen;
using Xunit;

namespace VT100.Tests.Fakes.TestsForFakes
{
    /// <summary>
    /// Check that the <see cref="RawTextExtractor"/> produces acceptable results.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class TestRawTextExtractor
    {
        private static string[] _testStrings =
        {
            "Simple string",
            $"{ColourAttribute.GetBackgroundAttribute(VtColour.Yellow)}Background Yellow then {ColourAttribute.GetBackgroundAttribute(VtColour.Red)}Red{ColourAttribute.GetBackgroundAttribute(VtColour.NoColourChange)}",
            $"{ColourAttribute.GetForegroundAttribute(VtColour.Blue)}Foreground Blue then {ColourAttribute.GetBackgroundAttribute(VtColour.Magenta)}Magenta{ColourAttribute.GetForegroundAttribute(VtColour.NoColourChange)}",
        };
        
        [Fact]
        public void TextIsExtracted()
        {
            //Act
            var result = _testStrings.Select(t => new {Input = t.Replace("\x1b", "^"), Result = RawTextExtractor.Extract(t)});

            //Assert
            var output = new Output();
            output.WrapLine("Note: ESC characters are replaced with ^ for display purposes");
            output.WriteLine();
            output.FormatTable(result);
            output.Report.Verify();
        }
    }
}