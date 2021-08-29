using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using VT100.Utilities.ReadConsole;
using Xunit;
// ReSharper disable InconsistentNaming

namespace VT100.Tests.Utilities.ReadConsole
{
    public class TestCodeAnalyser
    {
        [Theory]
        [InlineData(new [] {'C'}, ResolvedCode.CursorForward)]
        [InlineData(new [] {'D'}, ResolvedCode.CursorBackwards)]
        [InlineData(new [] {'A'}, ResolvedCode.CursorUp)]
        [InlineData(new [] {'B'}, ResolvedCode.CursorDown)]
        [InlineData(new [] {'1', '1', '~'}, ResolvedCode.PF1)]
        [InlineData(new [] {'1', 'P'}, ResolvedCode.PF1)]
        [InlineData(new [] {'1', '2', '~'}, ResolvedCode.PF2)]
        [InlineData(new [] {'1', 'Q'}, ResolvedCode.PF2)]
        [InlineData(new [] {'1', '3', '~'}, ResolvedCode.PF3)]
        [InlineData(new [] {'1', 'R'}, ResolvedCode.PF3)]
        [InlineData(new [] {'1', '4', '~'}, ResolvedCode.PF4)]
        [InlineData(new [] {'1', 'S'}, ResolvedCode.PF4)]
        [InlineData(new [] {'1', '7', '~'}, ResolvedCode.PF6)]
        [InlineData(new [] {'1', '8', '~'}, ResolvedCode.PF7)]
        [InlineData(new [] {'1', '9', '~'}, ResolvedCode.PF8)]
        [InlineData(new [] {'2', '0', '~'}, ResolvedCode.PF9)]
        [InlineData(new [] {'2', '1', '~'}, ResolvedCode.PF10)]
        [InlineData(new [] {'2', '3', '~'}, ResolvedCode.PF11)]
        [InlineData(new [] {'2', '4', '~'}, ResolvedCode.PF12)]
        [InlineData(new [] {'2', '5', '~'}, ResolvedCode.PF13)]
        [InlineData(new [] {'2', '6', '~'}, ResolvedCode.PF14)]
        [InlineData(new [] {'2', '8', '~'}, ResolvedCode.PF15)]
        [InlineData(new [] {'2', '9', '~'}, ResolvedCode.PF16)]
        [InlineData(new [] {'3', '1', '~'}, ResolvedCode.PF17)]
        [InlineData(new [] {'3', '2', '~'}, ResolvedCode.PF18)]
        [InlineData(new [] {'3', '3', '~'}, ResolvedCode.PF19)]
        [InlineData(new [] {'3', '4', '~'}, ResolvedCode.PF20)]
        [InlineData(new [] {'H'}, ResolvedCode.Home)]
        [InlineData(new [] {'F'}, ResolvedCode.End)]
        [InlineData(new [] {'3', '~'}, ResolvedCode.Delete)]
        [InlineData(new [] {'2', '~'}, ResolvedCode.Insert)]
        [InlineData(new [] {'6', '~'}, ResolvedCode.PageDown)]
        [InlineData(new [] {'E'}, ResolvedCode.Begin)] 
        [InlineData(new [] {'5', '~'}, ResolvedCode.PageUp)]  
        [InlineData(new [] {'Z'}, ResolvedCode.NotRecognised)]
        [InlineData(new [] {'9', ';', '5', 'R'}, ResolvedCode.CPR, "9, 5")]
        public void SimpleCSICodesAreRecognised(char[] input, ResolvedCode code, string parameterString = null)
        {
            // Arrange
            var testChars = new[] { '\x1b', '[' }.Concat(input);
            var seq = ToSequence(testChars);
            var type = AnsiCodeType.CSI;

            //Act
            var codeAnalyser = new CodeAnalyser(CodeAnalyserSettings.PreferPF3Modifiers);
            var result = codeAnalyser.Analyse(seq, type);

            //Assert
            (result.Code, string.Join(", ", result.Parameters)).Should().Be((code, parameterString ?? string.Empty));
        }

        [Theory]
        [InlineData(new [] {'P'}, ResolvedCode.PF1)]
        [InlineData(new [] {'Q'}, ResolvedCode.PF2)]
        [InlineData(new [] {'R'}, ResolvedCode.PF3)]
        [InlineData(new [] {'S'}, ResolvedCode.PF4)]
        [InlineData(new [] {'H'}, ResolvedCode.Home)]
        [InlineData(new [] {'F'}, ResolvedCode.End)]
        [InlineData(new [] {'M'}, ResolvedCode.CR)]
        [InlineData(new [] {'I'}, ResolvedCode.Tab)]
        [InlineData(new [] {' '}, ResolvedCode.Space)]
        [InlineData(new [] {'W'}, ResolvedCode.NotRecognised)]
        public void SimpleSS3CodesAreRecognised(char[] input, ResolvedCode code)
        {
            // Arrange
            var testChars = new[] { '\x1b', 'O' }.Concat(input);
            var seq = ToSequence(testChars);
            var type = AnsiCodeType.SS3;

            //Act
            var codeAnalyser = new CodeAnalyser();
            var result = codeAnalyser.Analyse(seq, type);

            //Assert
            (result.Code, string.Join(", ", result.Parameters)).Should().Be((code, ""));
        }

        [Theory]
        [InlineData('\t', ResolvedCode.Tab)]
        [InlineData('\x7f', ResolvedCode.Backspace)]
        [InlineData('\r', ResolvedCode.CR)]
        [InlineData('X', ResolvedCode.NotRecognised)]
        public void SingleCharCodesAreRecognised(char input, ResolvedCode code)
        {
            // Arrange
            var seq = ToSequence(new []{ input });
            var type = AnsiCodeType.None;

            //Act
            var codeAnalyser = new CodeAnalyser();
            var result = codeAnalyser.Analyse(seq, type);

            //Assert
            (result.Code, string.Join(", ", result.Parameters)).Should().Be((code, ""));
        }

        private List<ControlElement> ToSequence(IEnumerable<char> testChars)
        {
            return testChars.Select(c => new ControlElement { KeyChar = c }).ToList();
        }
    }
}
