using ConsoleToolkit;
using ConsoleToolkit.CommandLineInterpretation;
using NUnit.Framework;

namespace ConsoleToolkitTests
{
    [TestFixture]
    public class TestToolkitOptions
    {
        private ToolkitOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = new ToolkitOptions();
        }

        [Test]
        public void DefaultParserConventionsIsMicrosoftStandard()
        {
            Assert.That(_options.ParsingConventions, Is.EqualTo(CommandLineParserConventions.MicrosoftStandard));
        }

        [Test]
        public void DefaultConfirmationTextIsY()
        {
            Assert.That(_options.ConfirmationInfo.YesText, Is.EqualTo("Y"));
        }

        [Test]
        public void DefaultConfirmationPromptIsYes()
        {
            Assert.That(_options.ConfirmationInfo.YesPrompt, Is.EqualTo("Yes"));
        }

        [Test]
        public void DefaultConfirmationTextIsN()
        {
            Assert.That(_options.ConfirmationInfo.NoText, Is.EqualTo("N"));
        }

        [Test]
        public void DefaultConfirmationPromptIsNo()
        {
            Assert.That(_options.ConfirmationInfo.NoPrompt, Is.EqualTo("No"));
        }

        [Test]
        public void YesTextCanBeOverridden()
        {
            _options.OverrideConfirmOptions("T", "True", "F", "False");
            Assert.That(_options.ConfirmationInfo.YesText, Is.EqualTo("T"));
        }

        [Test]
        public void YesPromptCanBeOverridden()
        {
            _options.OverrideConfirmOptions("T", "True", "F", "False");
            Assert.That(_options.ConfirmationInfo.YesPrompt, Is.EqualTo("True"));
        }

        [Test]
        public void NoTextCanBeOverridden()
        {
            _options.OverrideConfirmOptions("T", "True", "F", "False");
            Assert.That(_options.ConfirmationInfo.NoText, Is.EqualTo("F"));
        }

        [Test]
        public void NoPromptCanBeOverridden()
        {
            _options.OverrideConfirmOptions("T", "True", "F", "False");
            Assert.That(_options.ConfirmationInfo.NoPrompt, Is.EqualTo("False"));
        }
    }
}