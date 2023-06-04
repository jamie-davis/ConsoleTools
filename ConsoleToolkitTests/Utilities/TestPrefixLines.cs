using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.Utilities;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.Utilities
{
    [UseReporter(typeof (CustomReporter))]
    public class TestPrefixLines
    {
        [Fact]
        public void SingleLineIsPrefixedWithNoNewLine()
        {
            //Arrange
            var line = "just this";

            //Act
            var output = PrefixLines.Do(line, "test:");

            //Assert
            Assert.Equal("test:just this", output);
        }

        [Fact]
        public void SetOfLinesIsPrefixed()
        {
            //Arrange
            var sb = new StringBuilder();
            sb.AppendLine("line 1");
            sb.AppendLine("line 2");
            sb.AppendLine("line 3");

            //Act
            var output = PrefixLines.Do(sb.ToString(), "test:");

            //Assert
            Approvals.Verify(output);
        }

        [Fact]
        public void NoExtraNewLineIsAddedToSetOfLinesWithNoTrailingNewLine()
        {
            //Arrange
            var sb = new StringBuilder();
            sb.AppendLine("line 1");
            sb.AppendLine("line 2");
            sb.Append("line 3");

            //Act
            var output = PrefixLines.Do(sb.ToString(), "test:");

            //Assert
            Approvals.Verify(output);
        }

        [Fact]
        public void EmptyStringRemainsEmpty()
        {
            //Act
            var output = PrefixLines.Do(string.Empty, "test:");

            //Assert
            Assert.Equal(string.Empty, output);
        }
    }
}
