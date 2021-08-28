using System.Linq;
using FluentAssertions;
using VT100.Utilities.ReadConsole;
using Xunit;

namespace VT100.Tests.Utilities.ReadConsole
{
    public class TestCodeSequenceParameterExtractor
    {
        [Theory]
        [InlineData("15;5~", "15~", "5")]
        [InlineData("15;~", "15~", "")]
        [InlineData("15~", "15~", "")]
        public void CSIParameterIsExtracted(string codeFragment, string expectedResultFragment, string expectedParameters)
        {
            //Arrange
            var controlSeq = $"\x1b[{codeFragment}"
                .Select(c => new ControlElement { Key = new KeyInfo { KeyChar = c } })
                .ToList();

            //Act
            var result = CodeSequenceParameterExtractor.Extract(controlSeq, AnsiCodeType.CSI);

            //Assert
            var rebuilt = new string(result.Sequence.Skip(2).Select(c => c.Key.KeyChar).ToArray());
            (rebuilt, string.Join(", ", result.Parameters)).Should().Be((expectedResultFragment, expectedParameters));
        }

    }
}
