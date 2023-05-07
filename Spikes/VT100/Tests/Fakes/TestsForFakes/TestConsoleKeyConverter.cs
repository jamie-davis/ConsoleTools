using System;
using System.Linq;
using System.Reflection.Metadata;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;

namespace VT100.Tests.Fakes.TestsForFakes
{
    /// <summary>
    /// Check that the <see cref="ConsoleKeyConverter"/> produces acceptable <see cref="ConsoleKey"/> values.
    /// </summary>
    public class TestConsoleKeyConverter
    {
        [Fact]
        public void CharsAreConvertedToEnumValues()
        {
            //Arrange
            var ranges = new (char Base, char Limit)[]
            {
                ('A', 'Z'),
                ('a', 'z'),
                ('0', '9'),
            };
            var input = ranges.SelectMany(range => Enumerable.Range(range.Base, range.Limit - range.Base))
                .Select(c => (char)c)
                .Concat("\t\r\n[](),.!\"£$%^&*+-_=@~#?/<>|\\':;")
                .ToList();

            //Act
            var result = input.Select(i => new { Input = i, Output = ConsoleKeyConverter.FromChar(i) }).ToList();

            //Assert
            var output = new Output();
            output.FormatTable(result);
            output.Report.Verify();
        }
    }
}