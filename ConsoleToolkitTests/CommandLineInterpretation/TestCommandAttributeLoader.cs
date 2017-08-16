using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
using Description = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;

namespace ConsoleToolkitTests.CommandLineInterpretation
{

    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandAttributeLoader
    {
        // ReSharper disable PossibleNullReferenceException
        private CommandConfig<DefaultName> _defaultNameCommand;

        #region Types for test
#pragma warning disable 649

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

        [Command("deftest")]
        class CommandWithDefaultedPositional
        {
            [Positional(0)]
            [Description("Positional field description")]
            public string PosField;

            [Positional(1, DefaultValue = "deffo")]
            [Description("Positional field description")]
            public string PosField2;

            [Positional(1, DefaultValue = null)]
            [Description("Positional field description")]
            public string PosField3;
        }

        [Command]
        class CommandWithDuplicateOptionName
        {
            [Option("LongName", "S")]
            public string Option1;

            [Option("LongName2", "S")]
            public string Option2;
        }

        [Command]
        class CommandWithShortCircuitOption
        {
            [Option]
            public bool A { get; set; }
            
            [Option]
            public bool B { get; set; }

            [Option(ShortCircuit = true)]
            public bool C { get; set; }
        }

        [Command]
        class SuffixTestCommand { }

        [Command]
        class RepeatingPositionalCommand
        {
            [Positional]
            public string Normal { get; set; }

            [Positional]
            public List<string> Repeating { get; set; }
        }

        [Command]
        class RepeatingOptionCommand
        {
            [Option]
            public string Normal { get; set; }

            [Option]
            public List<string> Repeating { get; set; }
        }

        [Command]
        class RepeatingListCommand
        {
            [Option]
            public string Normal { get; set; }

            [Option]
            public List<List<string>> Repeating { get; set; }
        }

        [Command]
        class RepeatingStructureCommand
        {
            public class Config
            {
                public string Key { get; set; }
                public string Value { get; set; }
            }

            [Option]
            public string Normal { get; set; }

            [Option]
            public List<Config> Repeating { get; set; }
        }


        class RepeatingOptions
        {
            [Option("dbnames", "d")]
            [Description("Database names")]
            public List<string> DbNames { get; set; }

            [Option("dbservers", "s")]
            [Description("Database servers")]
            public List<string> ServerName { get; set; }
        }

        [Command]
        class RepeatingOptionSetCommand
        {
            [Option]
            public string Normal { get; set; }

            [OptionSet]
            public RepeatingOptions CommandOptions { get; set; }
        }

        class CommonOptions
        {
            [Option("dbname", "d")]
            [Description("Database name")]
            public string DbName { get; set; }

            [Option("dbserver", "s")]
            [Description("Database server")]
            public string ServerName { get; set; }
        }

        [Command]
        class ExtendedCommand
        {
            [Positional]
            public string Group { get; set; }

            [Positional]
            public int AdditionalLength { get; set; }

            [Option("showprogress")]
            public bool DisplayProgress { get; set; }

            [OptionSet]
            public CommonOptions CommandOptions { get; set; }
        }

        [Command]
        class ValidatedCommand
        {
            [CommandValidator]
            public bool Valid(IList<string> errors)
            {
                errors.Add("error message");
                return false;
            }
        }

        [Command]
        class ThrowingValidatorCommand
        {
            [CommandValidator]
            public bool Valid()
            {
                throw new Exception("Thrown error message.");
            }
        }

        [Command]
        class MultiValidatorCommand
        {
            public bool NoParamValidatorCalled;
            public bool ErrorListValidatorCalled;
            private bool _failNoParamValidation;

            [CommandValidator]
            public bool Valid()
            {
                NoParamValidatorCalled = true;
                return _failNoParamValidation ? false : true;
            }

            [CommandValidator]
            public bool Valid(IList<string> errors)
            {
                ErrorListValidatorCalled = true;
                return true;
            }

            public void FailNoParamValidation()
            {
                _failNoParamValidation = true;
            }
        }

        [Command]
        [Keyword("keyword")]
        class CommandWithKeyword
        {
            
        }

        [Command]
        [Keyword("keyword1 keyword-2 keyword3")]
        class CommandWithMultipleKeywords
        {
            
        }

        // ReSharper restore UnusedField.Compiler
        // ReSharper restore UnusedMember.Local
#pragma warning restore 649
        #endregion

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
            Assert.That(_defaultNameCommand.Name, Is.EqualTo("DefaultName".ToLower()));
        }

        [Test]
        public void CommandIsDroppedFromDefaultCommandName()
        {
            var command = CommandAttributeLoader.Load(typeof (SuffixTestCommand));
            Assert.That(command.Name, Is.EqualTo("SuffixTest".ToLower()));
        }

        [Test]
        public void CommandNameCanBeSpecified()
        {
            var over = CommandAttributeLoader.Load(typeof (CommandWithNameOverride)) as CommandConfig<CommandWithNameOverride>;
            Assert.That(over.Name, Is.EqualTo("over"));
        }

        [Test]
        public void CommandKeywordIsLoaded()
        {
            var keywordCommand = CommandAttributeLoader.Load(typeof (CommandWithKeyword)) as CommandConfig<CommandWithKeyword>;
            Assert.That(keywordCommand.Keywords, Is.EqualTo(new [] {"keyword"}));
        }

        [Test]
        public void MultipleCommandKeywordIsLoaded()
        {
            var keywordCommand = CommandAttributeLoader.Load(typeof (CommandWithMultipleKeywords)) as CommandConfig<CommandWithMultipleKeywords>;
            Assert.That(keywordCommand.Keywords, Is.EqualTo(new [] {"keyword1", "keyword-2", "keyword3"}));
        }

        [Test]
        public void CommandDescriptionIsExtracted()
        {
            Assert.That((_defaultNameCommand as IContext).Description, Is.EqualTo("Command description"));
        }

        [Test]
        public void PositionalPropertyParametersAreExtracted()
        {
            Assert.That(_defaultNameCommand.Positionals.Any(p => p.ParameterName == "prop"));
        }

        [Test]
        public void PositionalFieldParametersAreExtracted()
        {
            Assert.That(_defaultNameCommand.Positionals.Any(p => p.ParameterName == "posfield"));
        }

        [Test]
        public void PositionalDescriptionIsExtracted()
        {
            var positional = _defaultNameCommand.Positionals.First(p => p.ParameterName == "posfield");
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
            Assert.That(_defaultNameCommand.Options.Any(o => o.Name == "optionprop"));
        }

        [Test]
        public void OptionFieldIsExtracted()
        {
            Assert.That(_defaultNameCommand.Options.Any(o => o.Name == "optionfield"));
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
        public void OptionShortCircuitIsLoaded()
        {
            var cmd = CommandAttributeLoader.Load(typeof(CommandWithShortCircuitOption)) as CommandConfig<CommandWithShortCircuitOption>;
            var option = cmd.Options.First(o => o.Name == "c");
            Assert.That(option.IsShortCircuit, Is.True);
        }

        [Test]
        public void OptionsDoNothaveShortCircuitByDefault()
        {
            var cmd = CommandAttributeLoader.Load(typeof(CommandWithShortCircuitOption)) as CommandConfig<CommandWithShortCircuitOption>;
            var option = cmd.Options.First(o => o.Name == "b");
            Assert.That(option.IsShortCircuit, Is.False);
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

        [Test]
        public void PositionalWithoutDefaultSetIsNotOptional()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals.First();
            Assert.That(nonDefPositional.IsOptional, Is.False);
        }

        [Test]
        public void PositionalWithDefaultSetIsOptional()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[1];
            Assert.That(nonDefPositional.IsOptional, Is.True);
        }

        [Test]
        public void PositionalWithDefaultSetHasDefaultValue()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[1];
            Assert.That(nonDefPositional.DefaultValue, Is.EqualTo("deffo"));
        }

        [Test]
        public void PositionalWithoutDefaultSetHasNoDefaultValue()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[0];
            Assert.That(nonDefPositional.DefaultValue, Is.Null);
        }

        [Test]
        public void PositionalWithNullDefaultIsOptional()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[2];
            Assert.That(nonDefPositional.IsOptional, Is.True);
        }

        [Test]
        public void PositionalWithNullDefaultHasCorrectDefaultValue()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[2];
            Assert.That(nonDefPositional.DefaultValue, Is.Null);
        }

        [Test]
        public void CommandImportsOptionSet()
        {
            var set = CommandAttributeLoader.Load(typeof(ExtendedCommand)) as CommandConfig<ExtendedCommand>;
            var options = set.Options.Select(o => o.Name).JoinWith(",");
            Assert.That(options, Is.EqualTo("showprogress,dbname,dbserver"));
        }

        [Test]
        public void RepeatingPositionalIsDetected()
        {
            var set = CommandAttributeLoader.Load(typeof(RepeatingPositionalCommand)) as CommandConfig<RepeatingPositionalCommand>;
            var positionals = set.Positionals
                .Select(pos => string.Format("{0}({1})", pos.ParameterName, pos.AllowMultiple))
                .JoinWith(",");
            Assert.That(positionals, Is.EqualTo("normal(False),repeating(True)"));
        }

        [Test]
        public void RepeatingPositionalsHaveListType()
        {
            var set = CommandAttributeLoader.Load(typeof(RepeatingPositionalCommand)) as CommandConfig<RepeatingPositionalCommand>;
            var positionals = set.Positionals
                .Select(pos => string.Format("{0}({1})", pos.ParameterName, pos.ParameterType))
                .JoinWith(",");
            Assert.That(positionals, Is.EqualTo("normal(System.String),repeating(System.String)"));
        }

        [Test]
        public void RepeatingPositionalsInsertValues()
        {
            var set = CommandAttributeLoader.Load(typeof(RepeatingPositionalCommand)) as CommandConfig<RepeatingPositionalCommand>;
            var positional = set.Positionals.FirstOrDefault(p => p.ParameterName == "repeating");
            var command = set.Create(null) as RepeatingPositionalCommand;
            positional.Accept(command, "First");
            positional.Accept(command, "Second");
            positional.Accept(command, "Third");

            var result = command.Repeating.JoinWith(",");
            Assert.That(result, Is.EqualTo("First,Second,Third"));
        }

        [Test]
        public void RepeatingOptionIsDetected()
        {
            var config = CommandAttributeLoader.Load(typeof(RepeatingOptionCommand)) as CommandConfig<RepeatingOptionCommand>;
            var positionals = config.Options
                .Select(pos => string.Format("{0}({1})", pos.Name, pos.AllowMultiple))
                .JoinWith(",");
            Assert.That(positionals, Is.EqualTo("normal(False),repeating(True)"));
        }

        [Test]
        public void RepeatingOptionsInsertValues()
        {
            var config = CommandAttributeLoader.Load(typeof(RepeatingOptionCommand)) as CommandConfig<RepeatingOptionCommand>;
            var option = config.Options.FirstOrDefault(p => p.Name == "repeating") as CommandOption<Action<RepeatingOptionCommand, string>>;
            var command = config.Create(null) as RepeatingOptionCommand;
            string error;
            option.Apply(command, new [] {"First"}, out error);
            option.Apply(command, new[] { "Second" }, out error);
            option.Apply(command, new[] { "Third" }, out error);

            var result = command.Repeating.JoinWith(",");
            Assert.That(result, Is.EqualTo("First,Second,Third"));
        }

        [Test, ExpectedException]
        public void RepeatingOptionTypeCannotBeCollection()
        {
            var config = CommandAttributeLoader.Load(typeof(RepeatingListCommand)) as CommandConfig<RepeatingListCommand>;
        }

        [Test]
        public void RepeatingComplexTypeInsertsValues()
        {
            var config = CommandAttributeLoader.Load(typeof(RepeatingStructureCommand)) as CommandConfig<RepeatingStructureCommand>;
            var option = config.Options.FirstOrDefault(p => p.Name == "repeating") as CommandOption<Action<RepeatingStructureCommand, string, string>>;
            var command = config.Create(null) as RepeatingStructureCommand;
            string error;
            option.Apply(command, new [] {"key1", "one"}, out error);
            option.Apply(command, new [] {"key2", "two"}, out error);
            option.Apply(command, new [] {"key3", "three"}, out error);

            var result = command.Repeating.Select(r => string.Format("[{0},{1}]", r.Key, r.Value)).JoinWith(",");
            Assert.That(result, Is.EqualTo("[key1,one],[key2,two],[key3,three]"));
        }

        [Test]
        public void RepeatingOptionSetOptionAcceptsValues()
        {
            var config = CommandAttributeLoader.Load(typeof(RepeatingOptionSetCommand)) as CommandConfig<RepeatingOptionSetCommand>;
            var option = config.Options.FirstOrDefault(p => p.Name == "dbnames") as CommandOption<Action<RepeatingOptionSetCommand, string>>;
            var command = config.Create(null) as RepeatingOptionSetCommand;
            string error;
            option.Apply(command, new [] {"one"}, out error);
            option.Apply(command, new [] {"two"}, out error);
            option.Apply(command, new [] {"three"}, out error);

            var result = command.CommandOptions.DbNames.JoinWith(",");
            Assert.That(result, Is.EqualTo("one,two,three"));
        }

        [Test]
        public void ValidationMethodCanBeSpecified()
        {
            var config = CommandAttributeLoader.Load(typeof(ValidatedCommand)) as CommandConfig<ValidatedCommand>;
            var command = config.Create(null) as ValidatedCommand;
            var errors = new List<string>();
            config.Validate(command, errors);
            Assert.That(errors, Is.EqualTo(new [] {"error message"}));
        }

        [Test]
        public void ValidationMethodCanThrowErrors()
        {
            var config = CommandAttributeLoader.Load(typeof(ThrowingValidatorCommand)) as CommandConfig<ThrowingValidatorCommand>;
            var command = config.Create(null) as ThrowingValidatorCommand;
            var errors = new List<string>();
            config.Validate(command, errors);
            Assert.That(errors, Is.EqualTo(new [] {"Thrown error message."}));
        }

        [Test]
        public void MultipleValidationMethodsCanBeSpecified()
        {
            var config = CommandAttributeLoader.Load(typeof(MultiValidatorCommand)) as CommandConfig<MultiValidatorCommand>;
            var command = config.Create(null) as MultiValidatorCommand;
            var errors = new List<string>();
            config.Validate(command, errors);
            Assert.That(command.ErrorListValidatorCalled && command.NoParamValidatorCalled, Is.True);
        }

        [Test]
        public void ValidatorCallsShortCircuit()
        {
            var config = CommandAttributeLoader.Load(typeof(MultiValidatorCommand)) as CommandConfig<MultiValidatorCommand>;
            var command = config.Create(null) as MultiValidatorCommand;
            var errors = new List<string>();
            command.FailNoParamValidation();
            config.Validate(command, errors);
            Assert.That(command.ErrorListValidatorCalled, Is.False);
        }
    }
}