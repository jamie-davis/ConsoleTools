using System;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
using Description = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandAttributeLoader
    {
        private CommandConfig<DefaultName> _defaultNameCommand;

        class InvalidBecauseNotACommand
        {
            
        }

        // ReSharper disable UnusedField.Compiler
        // ReSharper disable UnusedMember.Local
        [Command]
        [Description("Command description")]
        class DefaultName
        {
            [Positional("prop", 1)]
            [Description("Positional property description")]
            public string PosProp { get; set; }

            [Positional(0)]
            [Description("Positional field description")]
            public string PosField;

            public string NormalPropertyNotExtracted { get; set; }
            public string NormalFieldNotExtracted;

            [Option]
            [Description("Option property description")]
            public string OptionProp { get; set; }

            [Option]
            [Description("Option field description")]
            public string OptionField;

            [Option("CustomName")]
            [Description("Option with customised name")]
            public string OptionCustom;

            [Option("LongName", "S")]
            [Description("Option with long and short name")]
            public string WellNamedOption;

            [Option("Multi")]
            [Description("Option with multiple parameters")]
            public MultiOpt MultiOptParams { get; set; }

            [Option("switch")]
            [Description("Boolean option")]
            public bool BoolOption { get; set; }

            public class MultiOpt
            {
                [Positional(0)]
                public string StringParam { get; set; }

                [Positional(1)]
                public int IntParam { get; set; }
                
                [Positional(2)]
                public DateTime DateParam { get; set; }
            }

        }

        [Command("over")]
        class CommandWithNameOverride
        {
            [Positional(0)]
            [Description("Positional field description")]
            public string PosField;
        }

        [Command]
        class CommandWithDuplicateOptionName
        {
            [Option("LongName", "S")]
            public string Option1;

            [Option("LongName2", "S")]
            public string Option2;
        }
        // ReSharper restore UnusedField.Compiler
        // ReSharper restore UnusedMember.Local

        [SetUp]
        public void SetUp()
        {
            _defaultNameCommand = CommandAttributeLoader.Load(typeof (DefaultName)) as CommandConfig<DefaultName>;
        }

        [Test, ExpectedException]
        public void TypesThatAreNotCommandsCannotBeLoaded()
        {
            CommandAttributeLoader.Load(typeof(InvalidBecauseNotACommand));
        }

        [Test]
        public void DefaultCommandNameIsDerivedFromClass()
        {
            Assert.That(_defaultNameCommand.Name, Is.EqualTo("DefaultName"));
        }

        [Test]
        public void CommandNameCanBeSpecified()
        {
            var over = CommandAttributeLoader.Load(typeof (CommandWithNameOverride)) as CommandConfig<CommandWithNameOverride>;
            Assert.That(over.Name, Is.EqualTo("over"));
        }

        [Test]
        public void CommandDescriptionIsExrtacted()
        {
            Assert.That((_defaultNameCommand as BaseCommandConfig).Description, Is.EqualTo("Command description"));
        }

        [Test]
        public void PositionalPropertyParametersAreExtracted()
        {
            Assert.That(_defaultNameCommand.Positionals.Any(p => p.ParameterName == "prop"));
        }

        [Test]
        public void PositionalFieldParametersAreExtracted()
        {
            Assert.That(_defaultNameCommand.Positionals.Any(p => p.ParameterName == "PosField"));
        }

        [Test]
        public void PositionalDescriptionIsExtracted()
        {
            var positional = _defaultNameCommand.Positionals.First(p => p.ParameterName == "PosField");
            Assert.That(positional.Description, Is.Not.Null);
        }

        [Test]
        public void UnattributedFieldsAndPropertiesAreNotExtracted()
        {
            var allNames = _defaultNameCommand.Positionals
                .Select(p => p.ParameterName)
                .Concat(_defaultNameCommand.Options.Select(o => o.Name))
                .ToList();
            Assert.That(allNames.Count(n => n.Contains("Normal")), Is.EqualTo(0));
        }

        [Test]
        public void OptionPropertyIsExtracted()
        {
            Assert.That(_defaultNameCommand.Options.Any(o => o.Name == "OptionProp"));
        }

        [Test]
        public void OptionFieldIsExtracted()
        {
            Assert.That(_defaultNameCommand.Options.Any(o => o.Name == "OptionField"));
        }

        [Test]
        public void MultiParamOptionIsExtracted()
        {
            Assert.That(_defaultNameCommand.Options.Any(o => o.Name == "Multi"));
        }

        [Test]
        public void OptionNameCanBeOverridden()
        {
            Assert.That(_defaultNameCommand.Options.Any(o => o.Name == "CustomName"));
        }

        [Test]
        public void OptionLongAndShortNamesCanBeSpecified()
        {
            var option = _defaultNameCommand.Options.First(o => o.Name == "LongName");
            Assert.That(option.Aliases.Any(a => a == "S"));
        }

        [Test]
        public void BooleanOptionHasIsBooleanSet()
        {
            var option = _defaultNameCommand.Options.First(o => o.Name == "switch");
            Assert.That(option.IsBoolean, Is.True);
        }

        [Test, ExpectedException(typeof (DuplicateOptionName))]
        public void CommandWithDuplicateOptionNameThrowsOnLoad()
        {
            CommandAttributeLoader.Load(typeof (CommandWithDuplicateOptionName));
        }
    }
}