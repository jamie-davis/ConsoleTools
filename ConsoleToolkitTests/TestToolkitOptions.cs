using ConsoleToolkit;
using ConsoleToolkit.CommandLineInterpretation;
using Xunit;

namespace ConsoleToolkitTests
{
    public class TestToolkitOptions
    {
        private ToolkitOptions _options;
        public TestToolkitOptions()
        {
            _options = new ToolkitOptions();
        }

        [Fact]
        public void DefaultParserConventionsIsMicrosoftStandard()
        {
            Assert.Equal(CommandLineParserConventions.MicrosoftStandard, _options.ParsingConventions);
        }

        [Fact]
        public void DefaultConfirmationTextIsY()
        {
            Assert.Equal("Y", _options.ConfirmationInfo.YesText);
        }

        [Fact]
        public void DefaultConfirmationPromptIsYes()
        {
            Assert.Equal("Yes", _options.ConfirmationInfo.YesPrompt);
        }

        [Fact]
        public void DefaultConfirmationTextIsN()
        {
            Assert.Equal("N", _options.ConfirmationInfo.NoText);
        }

        [Fact]
        public void DefaultConfirmationPromptIsNo()
        {
            Assert.Equal("No", _options.ConfirmationInfo.NoPrompt);
        }

        [Fact]
        public void YesTextCanBeOverridden()
        {
            _options.OverrideConfirmOptions("T", "True", "F", "False");
            Assert.Equal("T", _options.ConfirmationInfo.YesText);
        }

        [Fact]
        public void YesPromptCanBeOverridden()
        {
            _options.OverrideConfirmOptions("T", "True", "F", "False");
            Assert.Equal("True", _options.ConfirmationInfo.YesPrompt);
        }

        [Fact]
        public void NoTextCanBeOverridden()
        {
            _options.OverrideConfirmOptions("T", "True", "F", "False");
            Assert.Equal("F", _options.ConfirmationInfo.NoText);
        }

        [Fact]
        public void NoPromptCanBeOverridden()
        {
            _options.OverrideConfirmOptions("T", "True", "F", "False");
            Assert.Equal("False", _options.ConfirmationInfo.NoPrompt);
        }
    }
}