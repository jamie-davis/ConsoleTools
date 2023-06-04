using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
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

        [GlobalOptions]
        static class GlobalOptionsDefinition
        {

            [Option]
            [Description("Option property description")]
            public static string OptionProp { get; set; }

            [Option]
            [Description("Option field description")]
            public static string OptionField;

            [Option("CustomName")]
            [Description("Option with customised name")]
            public static string OptionCustom;

            [Option("LongName", "S")]
            [Description("Option with long and short name")]
            public static string WellNamedOption;

            [Option("Multi")]
            [Description("Option with multiple parameters")]
            public static DefaultName.MultiOpt MultiOptParams { get; set; }

            [Option("switch")]
            [Description("Boolean option")]
            public static bool BoolOption { get; set; }

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

        [GlobalOptions]
        class InstanceGlobalOptionsDefinition
        {
            [Option("env")]
            public string Environment { get; set; }

        }

        [GlobalOptions]
        static class GlobalOptionsWithDuplicateOptionName
        {
            [Option("LongName", "S")]
            public static string Option1;

            [Option("LongName2", "S")]
            public static string Option2;
        }

        [GlobalOptions]
        static class GlobalOptionsWithShortCircuitOption
        {
            [Option]
            public static bool A { get; set; }

            [Option]
            public static bool B { get; set; }

            [Option(ShortCircuit = true)]
            public static bool C { get; set; }
        }
        #endregion

        public TestCommandAttributeLoader()
        {
            _defaultNameCommand = CommandAttributeLoader.Load(typeof (DefaultName)) as CommandConfig<DefaultName>;
        }

        [Fact]
        public void TypesThatAreNotCommandsCannotBeLoaded()
        {
            var act = new Action(() => CommandAttributeLoader.Load(typeof(InvalidBecauseNotACommand)));
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void DefaultCommandNameIsDerivedFromClass()
        {
            Assert.Equal("DefaultName".ToLower(), _defaultNameCommand.Name);
        }

        [Fact]
        public void CommandIsDroppedFromDefaultCommandName()
        {
            var command = CommandAttributeLoader.Load(typeof (SuffixTestCommand));
            Assert.Equal("SuffixTest".ToLower(), command.Name);
        }

        [Fact]
        public void CommandNameCanBeSpecified()
        {
            var over = CommandAttributeLoader.Load(typeof (CommandWithNameOverride)) as CommandConfig<CommandWithNameOverride>;
            Assert.Equal("over", over.Name);
        }

        [Fact]
        public void CommandKeywordIsLoaded()
        {
            var keywordCommand = CommandAttributeLoader.Load(typeof (CommandWithKeyword)) as CommandConfig<CommandWithKeyword>;
            Assert.Equal(new[] { "keyword" }, keywordCommand.Keywords);
        }

        [Fact]
        public void MultipleCommandKeywordIsLoaded()
        {
            var keywordCommand = CommandAttributeLoader.Load(typeof (CommandWithMultipleKeywords)) as CommandConfig<CommandWithMultipleKeywords>;
            Assert.Equal(new[] { "keyword1", "keyword-2", "keyword3" }, keywordCommand.Keywords);
        }

        [Fact]
        public void CommandDescriptionIsExtracted()
        {
            Assert.Equal("Command description", (_defaultNameCommand as IContext).Description);
        }

        [Fact]
        public void PositionalPropertyParametersAreExtracted()
        {
            _defaultNameCommand.Positionals.Any(p => p.ParameterName == "prop").Should().BeTrue();
        }

        [Fact]
        public void PositionalFieldParametersAreExtracted()
        {
            _defaultNameCommand.Positionals.Any(p => p.ParameterName == "posfield").Should().BeTrue();
        }

        [Fact]
        public void PositionalDescriptionIsExtracted()
        {
            var positional = _defaultNameCommand.Positionals.First(p => p.ParameterName == "posfield");
            positional.Description.Should().NotBeNull();
        }

        [Fact]
        public void UnattributedFieldsAndPropertiesAreNotExtracted()
        {
            var allNames = _defaultNameCommand.Positionals
                .Select(p => p.ParameterName)
                .Concat(_defaultNameCommand.Options.Select(o => o.Name))
                .ToList();
            allNames.Count(n => n.Contains("Normal")).Should().Be(0);
        }

        [Fact]
        public void OptionPropertyIsExtracted()
        {
            _defaultNameCommand.Options.Any(o => o.Name == "optionprop").Should().BeTrue();
        }

        [Fact]
        public void OptionFieldIsExtracted()
        {
            _defaultNameCommand.Options.Any(o => o.Name == "optionfield").Should().BeTrue();
        }

        [Fact]
        public void MultiParamOptionIsExtracted()
        {
            _defaultNameCommand.Options.Any(o => o.Name == "Multi").Should().BeTrue();
        }

        [Fact]
        public void OptionNameCanBeOverridden()
        {
            _defaultNameCommand.Options.Any(o => o.Name == "CustomName").Should().BeTrue();
        }

        [Fact]
        public void OptionLongAndShortNamesCanBeSpecified()
        {
            var option = _defaultNameCommand.Options.First(o => o.Name == "LongName");
            option.Aliases.Any(a => a == "S").Should().BeTrue();
        }

        [Fact]
        public void OptionShortCircuitIsLoaded()
        {
            var cmd = CommandAttributeLoader.Load(typeof(CommandWithShortCircuitOption)) as CommandConfig<CommandWithShortCircuitOption>;
            var option = cmd.Options.First(o => o.Name == "c");
            option.IsShortCircuit.Should().BeTrue();
        }

        [Fact]
        public void OptionsDoNothaveShortCircuitByDefault()
        {
            var cmd = CommandAttributeLoader.Load(typeof(CommandWithShortCircuitOption)) as CommandConfig<CommandWithShortCircuitOption>;
            var option = cmd.Options.First(o => o.Name == "b");
            option.IsShortCircuit.Should().BeFalse();
        }

        [Fact]
        public void BooleanOptionHasIsBooleanSet()
        {
            var option = _defaultNameCommand.Options.First(o => o.Name == "switch");
            option.IsBoolean.Should().BeTrue();
        }

        [Fact]
        public void CommandWithDuplicateOptionNameThrowsOnLoad()
        {
            Assert.Throws<DuplicateOptionName>(() => CommandAttributeLoader.Load(typeof (CommandWithDuplicateOptionName)));
        }

        [Fact]
        public void PositionalWithoutDefaultSetIsNotOptional()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals.First();
            nonDefPositional.IsOptional.Should().BeFalse();
        }

        [Fact]
        public void PositionalWithDefaultSetIsOptional()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[1];
            nonDefPositional.IsOptional.Should().BeTrue();
        }

        [Fact]
        public void PositionalWithDefaultSetHasDefaultValue()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[1];
            Assert.Equal("deffo", nonDefPositional.DefaultValue);
        }

        [Fact]
        public void PositionalWithoutDefaultSetHasNoDefaultValue()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[0];
            nonDefPositional.DefaultValue.Should().BeNull();
        }

        [Fact]
        public void PositionalWithNullDefaultIsOptional()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[2];
            nonDefPositional.IsOptional.Should().BeTrue();
        }

        [Fact]
        public void PositionalWithNullDefaultHasCorrectDefaultValue()
        {
            var deffo = CommandAttributeLoader.Load(typeof(CommandWithDefaultedPositional)) as CommandConfig<CommandWithDefaultedPositional>;
            var nonDefPositional = deffo.Positionals[2];
            nonDefPositional.DefaultValue.Should().BeNull();
        }

        [Fact]
        public void CommandImportsOptionSet()
        {
            var set = CommandAttributeLoader.Load(typeof(ExtendedCommand)) as CommandConfig<ExtendedCommand>;
            var options = set.Options.Select(o => o.Name).JoinWith(",");
            Assert.Equal("showprogress,dbname,dbserver", options);
        }

        [Fact]
        public void RepeatingPositionalIsDetected()
        {
            var set = CommandAttributeLoader.Load(typeof(RepeatingPositionalCommand)) as CommandConfig<RepeatingPositionalCommand>;
            var positionals = set.Positionals
                .Select(pos => string.Format("{0}({1})", pos.ParameterName, pos.AllowMultiple))
                .JoinWith(",");
            Assert.Equal("normal(False),repeating(True)", positionals);
        }

        [Fact]
        public void RepeatingPositionalsHaveListType()
        {
            var set = CommandAttributeLoader.Load(typeof(RepeatingPositionalCommand)) as CommandConfig<RepeatingPositionalCommand>;
            var positionals = set.Positionals
                .Select(pos => string.Format("{0}({1})", pos.ParameterName, pos.ParameterType))
                .JoinWith(",");
            Assert.Equal("normal(System.String),repeating(System.String)", positionals);
        }

        [Fact]
        public void RepeatingPositionalsInsertValues()
        {
            var set = CommandAttributeLoader.Load(typeof(RepeatingPositionalCommand)) as CommandConfig<RepeatingPositionalCommand>;
            var positional = set.Positionals.FirstOrDefault(p => p.ParameterName == "repeating");
            var command = set.Create(null) as RepeatingPositionalCommand;
            positional.Accept(command, "First");
            positional.Accept(command, "Second");
            positional.Accept(command, "Third");

            var result = command.Repeating.JoinWith(",");
            Assert.Equal("First,Second,Third", result);
        }

        [Fact]
        public void RepeatingOptionIsDetected()
        {
            var config = CommandAttributeLoader.Load(typeof(RepeatingOptionCommand)) as CommandConfig<RepeatingOptionCommand>;
            var positionals = config.Options
                .Select(pos => string.Format("{0}({1})", pos.Name, pos.AllowMultiple))
                .JoinWith(",");
            Assert.Equal("normal(False),repeating(True)", positionals);
        }

        [Fact]
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
            Assert.Equal("First,Second,Third", result);
        }

        [Fact]
        public void RepeatingOptionTypeCannotBeCollection()
        {
            var act = new Action(() => CommandAttributeLoader.Load(typeof(RepeatingListCommand)));
            act.Should().Throw<Exception>();
        }

        [Fact]
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
            Assert.Equal("[key1,one],[key2,two],[key3,three]", result);
        }

        [Fact]
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
            Assert.Equal("one,two,three", result);
        }

        [Fact]
        public void ValidationMethodCanBeSpecified()
        {
            var config = CommandAttributeLoader.Load(typeof(ValidatedCommand)) as CommandConfig<ValidatedCommand>;
            var command = config.Create(null) as ValidatedCommand;
            var errors = new List<string>();
            config.Validate(command, errors);
            Assert.Equal(new[] { "error message" }, errors);
        }

        [Fact]
        public void ValidationMethodCanThrowErrors()
        {
            var config = CommandAttributeLoader.Load(typeof(ThrowingValidatorCommand)) as CommandConfig<ThrowingValidatorCommand>;
            var command = config.Create(null) as ThrowingValidatorCommand;
            var errors = new List<string>();
            config.Validate(command, errors);
            Assert.Equal(new[] { "Thrown error message." }, errors);
        }

        [Fact]
        public void MultipleValidationMethodsCanBeSpecified()
        {
            var config = CommandAttributeLoader.Load(typeof(MultiValidatorCommand)) as CommandConfig<MultiValidatorCommand>;
            var command = config.Create(null) as MultiValidatorCommand;
            var errors = new List<string>();
            config.Validate(command, errors);
            (command.ErrorListValidatorCalled && command.NoParamValidatorCalled).Should().BeTrue();
        }

        [Fact]
        public void ValidatorCallsShortCircuit()
        {
            var config = CommandAttributeLoader.Load(typeof(MultiValidatorCommand)) as CommandConfig<MultiValidatorCommand>;
            var command = config.Create(null) as MultiValidatorCommand;
            var errors = new List<string>();
            command.FailNoParamValidation();
            config.Validate(command, errors);
            command.ErrorListValidatorCalled.Should().BeFalse();
        }

        [Fact]
        public void GlobalOptionsAreLoaded()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsDefinition));
            config.Should().NotBeNull();
        }

        [Fact]
        public void GlobalOptionsMustBeStatic()
        {
            Assert.Throws<ArgumentException>(() => CommandAttributeLoader.LoadGlobalOptions(typeof(InstanceGlobalOptionsDefinition)));
        }

        [Fact]
        public void GlobalOptionPropertyIsExtracted()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsDefinition));
            config.Options.Any(o => o.Name == "optionprop").Should().BeTrue();
        }

        [Fact]
        public void GlobalOptionFieldIsExtracted()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsDefinition));
            config.Options.Any(o => o.Name == "optionfield").Should().BeTrue();
        }

        [Fact]
        public void GlobalMultiParamOptionIsExtracted()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsDefinition));
            config.Options.Any(o => o.Name == "Multi").Should().BeTrue();
        }

        [Fact]
        public void GlobalOptionNameCanBeOverridden()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsDefinition));
            config.Options.Any(o => o.Name == "CustomName").Should().BeTrue();
        }

        [Fact]
        public void GlobalOptionLongAndShortNamesCanBeSpecified()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsDefinition));
            var option = config.Options.First(o => o.Name == "LongName");
            option.Aliases.Any(a => a == "S").Should().BeTrue();
        }

        [Fact]
        public void GlobalOptionShortCircuitIsLoaded()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsWithShortCircuitOption));
            var option = config.Options.First(o => o.Name == "c");
            option.IsShortCircuit.Should().BeTrue();
        }

        [Fact]
        public void GlobalOptionsDoNothaveShortCircuitByDefault()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsDefinition));
            var option = config.Options.First(o => o.Name == "LongName");
            option.IsShortCircuit.Should().BeFalse();
        }

        [Fact]
        public void GlobalBooleanOptionHasIsBooleanSet()
        {
            var config = CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsDefinition));
            var option = config.Options.First(o => o.Name == "switch");
            option.IsBoolean.Should().BeTrue();
        }

        [Fact]
        public void GlobalConfigWithDuplicateOptionNameThrowsOnLoad()
        {
            Assert.Throws<DuplicateOptionName>(() => CommandAttributeLoader.LoadGlobalOptions(typeof(GlobalOptionsWithDuplicateOptionName)));
        }
    }
}