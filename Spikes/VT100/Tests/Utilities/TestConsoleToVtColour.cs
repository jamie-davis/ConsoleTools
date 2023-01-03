using System;
using System.Linq;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.FullScreen;
using VT100.Utilities;
using Xunit;

namespace VT100.Tests.Utilities
{
    public class TestConsoleToVtColour
    {
        [Fact]
        public void AllConsoleColoursAreConverted()
        {
            //Arrange
            var consoleColours = typeof(ConsoleColor).GetFields()
                .Where(f => f.IsLiteral)
                .Select(c => new { Colour = c.Name, Value = (int?)c.GetRawConstantValue() ?? -1 });
            
            //Act
            var result = consoleColours
                .Select(c => new 
                    { 
                        ConsoleColor = c.Colour, 
                        VtColour = ConsoleToVtColour.Convert((ConsoleColor)c.Value) 
                    });
            
            //Assert
            var output = new Output();
            output.FormatTable(result);
            output.Report.Verify();
        }
    }
}