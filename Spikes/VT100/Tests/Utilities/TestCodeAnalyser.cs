using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using VT100.Utilities.ReadConsole;
using Xunit;

namespace VT100.Tests.Utilities
{
    public class TestCodeAnalyser
    {
        [Theory]
        [InlineData(new [] {'C'}, ResolvedCode.CursorForward)]
        [InlineData(new [] {'D'}, ResolvedCode.CursorBackwards)]
        [InlineData(new [] {'A'}, ResolvedCode.CursorUp)]
        [InlineData(new [] {'B'}, ResolvedCode.CursorDown)]
        [InlineData(new [] {'1', '5', '~'}, ResolvedCode.PF5)]
        [InlineData(new [] {'1', '7', '~'}, ResolvedCode.PF6)]
        [InlineData(new [] {'1', '8', '~'}, ResolvedCode.PF7)]
        [InlineData(new [] {'1', '9', '~'}, ResolvedCode.PF8)]
        [InlineData(new [] {'2', '0', '~'}, ResolvedCode.PF9)]
        [InlineData(new [] {'2', '1', '~'}, ResolvedCode.PF10)]
        [InlineData(new [] {'2', '3', '~'}, ResolvedCode.PF11)]
        [InlineData(new [] {'2', '4', '~'}, ResolvedCode.PF12)]
        [InlineData(new [] {'H'}, ResolvedCode.Home)]
        [InlineData(new [] {'F'}, ResolvedCode.End)]
        [InlineData(new [] {'Z'}, ResolvedCode.NotRecognised)]
        public void SimpleCSICodesAreRecognised(char[] input, ResolvedCode code)
        {
            // Arrange
            var testChars = new[] { '\x1b', '[' }.Concat(input);
            var seq = ToSequence(testChars);
            var type = AnsiCodeType.CSI;

            //Act
            var result = CodeAnalyser.Analyse(seq, type);

            //Assert
            result.Should().Be(code);
        }

        [Theory]
        [InlineData(new [] {'P'}, ResolvedCode.PF1)]
        [InlineData(new [] {'Q'}, ResolvedCode.PF2)]
        [InlineData(new [] {'R'}, ResolvedCode.PF3)]
        [InlineData(new [] {'S'}, ResolvedCode.PF4)]
        [InlineData(new [] {'H'}, ResolvedCode.Home)]
        [InlineData(new [] {'F'}, ResolvedCode.End)]
        public void SimpleSS3CodesAreRecognised(char[] input, ResolvedCode code)
        {
            // Arrange
            var testChars = new[] { '\x1b', 'O' }.Concat(input);
            var seq = ToSequence(testChars);
            var type = AnsiCodeType.SS3;

            //Act
            var result = CodeAnalyser.Analyse(seq, type);

            //Assert
            result.Should().Be(code);
        }

        private List<ControlElement> ToSequence(IEnumerable<char> testChars)
        {
            return testChars.Select(c => new ControlElement { Key = new KeyInfo { KeyChar = c }}).ToList();
        }
    }
}
