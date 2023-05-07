using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.Utilities.ReadConsole;
using Xunit;

namespace VT100.Tests.Fakes.TestsForFakes
{
    /// <summary>
    /// Check that the <see cref="ControlElementExtractor"/> produces acceptable <see cref="ControlElement"/> values.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class TestControlElementExtractor
    {
        [Fact]
        public void ControlElementsAreExtracted()
        {
            //Arrange
            const string input = "abcdefgABCDEFG123456\t!£$\r\n";
            
            //Act
            var result = ControlElementExtractor.ListFromString(input);

            //Assert
            var output = new Output();
            output.WrapLine("Input text:");
            output.WriteLine();
            output.WrapLine(input);
            output.WriteLine();
            output.WriteLine();
            output.WrapLine("Result:");
            output.WriteLine();
            output.FormatTable(result.Select(c => new { c.KeyChar, Code = $"0x{(int)c.KeyChar:X2}", c.Key, c.Modifiers }));
            output.Report.Verify();
        }
    }
}