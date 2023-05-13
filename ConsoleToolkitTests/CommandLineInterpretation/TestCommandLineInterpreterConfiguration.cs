using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Exceptions;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;

// ReSharper disable once UnusedVariable

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [UseReporter(typeof(CustomReporter))]
    public class TestCommandLineInterpreterConfiguration
    {
        private CommandLineInterpreterConfiguration _config;
        private CustomParser _customParser;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;
        private static string _applicationName = "TestApp";

        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // ReSharper disable UnusedMember.Local
        public class CustomParser : ICommandLineParser
        {
            public void Parse(string[] args, IEnumerable<IOption> options,
                IEnumerable<IPositionalArgument> positionalArguments, IParserResult result)
            {
                throw new NotImplementedException();
            }
        }

        public class TestCommand
        {
            public string StringProp { get; set; }
            public bool BoolProp { get; set; }
            public int IntProp { get; set; }
        }

        public class MultiCaseCommand
        {
            public int Aone { get; set; }
            public string AOne { get; set; }
        }

        public class CustomParamCommand
        {
            CustomParamType Custom { get; set; }
        }

        public class CustomParamType
        {
            public string Value { get; private set; }

            public CustomParamType(char c, string name)
            {
                Value = c + "-" + name;
            }
        }
        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        public TestCommandLineInterpreterConfiguration()
        {
            _config = new CommandLineInterpreterConfiguration();
            _config.Command("first", s => new TestCommand())
                .Description("Description of the first commmand.");
            _config
                .Command("second", s => new TestCommand())
                .Description("The second command is a command with a number of parameters.")
                .Positional<string>("dateofthing", (command, s) => { })
                .Description("The date the thing should have.")
                .Positional<string>("numberofthing", (command, s) => { })
                .Description("The number of things that should be.");
            _config
                .Command("third", s => new TestCommand())
                .Description("The third command has a number of options but no parameters.")
                .Option("on", (command, b) => { })
                .Description("A simple option with no argument.")
                .Option<string, int>("fiddly", (command, s, n) => { })
                .Alias("f")
                .Description("An option with two arguments. The arguments need to be described in the text.");
            _config
                .Command("fourth", s => new TestCommand())
                .Description(
                    "The fourth command is really complicated with a number of parameters and also options. This is the sort of command that needs lots of text.")
                .Positional<string>("date", (command, s) => { })
                .Description("The date the complicated nonsense should be forgotten.")
                .Positional<string>("crpyticnum", (command, s) => { })
                .Description("The amount of nonsense the user needs to forget.")
                .Option("ignore", (command, b) => { })
                .Description("Use this option to consign this command to history, where it belongs.")
                .Option<string, int>("more", (command, s, n) => { })
                .Description("Even more.");

            _config
                .Command("desc", s => new TestCommand())
                .Description(
                    @"Descriptions can contain embedded line breaks -->
<-- like that one. These should be respected in the formatting. (This is meant to look a bit odd. Also, you should be aware that the deliberate line break is the only one in this text.)")
                .Positional<string>("pos", (command, s) => { })
                .Description(@"A parameter with
a line break.")
                .Option("lb", (command, b) => { })
                .Description("Another\nbreak.");

            _config
                .Command("exp", s => new TestCommand())
                .Description(@"Command with a positional and options configured using a Linq Expression, not a lambda.")
                .Positional("pos", command => command.StringProp)
                .Description(@"A positional configured with an expression.")
                .Option("B", command => command.BoolProp)
                .Description("A boolean option configured with an expression.")
                .Option("I", command => command.IntProp)
                .Description("A boolean option configured with an expression.");

            _config
                .Command("go", s => new TestCommand())
                .Keyword("keyword",
                    "Help text for the keyword(s). This text will be presented next to all of the keywords on which it it defined.")
                .Description(@"Command with a keyword.")
                .Positional("pos", command => command.StringProp)
                .Description(@"A positional configured with an expression.");

            _customParser = new CustomParser();

            _consoleOutInterface = new ConsoleInterfaceForTesting();
            _console = new ConsoleAdapter(_consoleOutInterface);

            _console.WriteLine(RulerFormatter.MakeRuler(40));
        }

        [Fact]
        public void InvalidOptionParameterTypeThrows()
        {
            void Call() =>
                new CommandLineInterpreterConfiguration()
                    .Command("test", s => s)
                    .Option<XDocument>("opt", (s, x) => { });

            Assert.Throws<InvalidParameterType>(Call);
        }
    
        [Fact]
        public void OptionsCanHaveAliases()
        {
            new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { })
                .Alias("optalias");
        }

        [Fact]
        public void OptionsAliasSameAsOptionNameThrows()
        {
            void Call() =>
            new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { })
                .Alias("opt");

            Assert.Throws<DuplicateOptionName>(Call);
        }

        [Fact]
        public void DuplicateOptionAliasThrows()
        {
            void Call() =>
            new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { })
                .Alias("o")
                .Alias("o");

            Assert.Throws<DuplicateOptionName>(Call);
        }

        [Fact]
        public void AliasSameAsOtherOptionNameThrows()
        {
            void Call() =>
                new CommandLineInterpreterConfiguration()
                    .Command("test", s => s)
                    .Option<string>("opt", (s, x) => { })
                    .Option<string>("opt2", (s, x) => { })
                    .Alias("opt");

            Assert.Throws<DuplicateOptionName>(Call);
        }

        [Fact]
        public void AliasSameAsOtherOptionAliasThrows()
        {
            void Call() =>
                new CommandLineInterpreterConfiguration()
                    .Command("test", s => s)
                    .Option<string>("opt", (s, x) => { })
                    .Alias("alias")
                    .Option<string>("opt2", (s, x) => { })
                    .Alias("alias");

            Assert.Throws<DuplicateOptionName>(Call);
        }

        [Fact]
        public void CommandAliasThrows()
        {
            void Call() =>
                new CommandLineInterpreterConfiguration()
                    .Command("test", s => s)
                    .Alias("invalid")
                    .Option<string>("opt", (s, x) => { });

            Assert.Throws<AliasNotSupported>(Call);
        }

        [Fact]
        public void PositionalAliasThrows()
        {
            void Call() =>
                new CommandLineInterpreterConfiguration()
                    .Command("test", s => s)
                    .Positional<string>("pos", (c, b) => { })
                    .Alias("invalid")
                    .Option<string>("opt", (s, x) => { });

            Assert.Throws<AliasNotSupported>(Call);
        }

        [Fact]
        public void ConfiguringTheSameCommandTwiceThrows()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { });

            void Call() =>
                config
                    .Command("test", s => s)
                    .Option<string>("opt", (s, x) => { });

            Assert.Throws<CommandAlreadySpecified>(Call);
        }

        [Fact]
        public void InvalidCommandParameterTypeThrows()
        {
            void Call() =>
                new CommandLineInterpreterConfiguration()
                    .Parameters(() => "test")
                    .Positional<XDocument>("opt", (s, x) => { });

            Assert.Throws<InvalidParameterType>(Call);
        }

        [Fact]
        public void ConfiguringDefaultCommandTwiceThrows()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => "test")
                .Positional<string>("opt", (s, x) => { });

            void Call() =>
                config
                    .Parameters(() => "test again")
                    .Positional<int>("opt2", (s, x) => { });

            Assert.Throws<ProgramParametersAlreadySpecified>(Call);
        }

        [Fact]
        public void DescriptionOfCommandsIsFormatted()
        {
            CommandDescriber.Describe(_config, _console, _applicationName, CommandExecutionMode.CommandLine);
            var description = _consoleOutInterface.GetBuffer();
            Console.WriteLine(description);
            Approvals.Verify(description);
        }

        [Fact]
        public void DefaultCommandHelpIsFormatted()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new TestCommand())
                .Description("Description of the whole program.")
                .Positional<string>("pos", (command, s) => { })
                    .Description("A positional parameter.");

            CommandDescriber.Describe(config, _console, _applicationName, CommandExecutionMode.CommandLine);
            var description = _consoleOutInterface.GetBuffer();
            Console.WriteLine(description);
            Approvals.Verify(description);
        }

        [Fact]
        public void ACustomConverterCanBeSpecifiedAndUsed()
        {
            CommandLineInterpreterConfiguration.AddCustomConverter(s => s.Length > 1 ? new CustomParamType(s.First(), s.Substring(1)) : null);
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new CustomParamCommand())
                .Description("Description of the whole program.")
                .Positional<CustomParamType>("pos", (command, s) => { })
                    .Description("A positional parameter.");

            CommandDescriber.Describe(config, _console, _applicationName, CommandExecutionMode.CommandLine);
            var description = _consoleOutInterface.GetBuffer();
            Console.WriteLine(description);
            Approvals.Verify(description);
        }

        [Fact]
        public void AValidationCanBeSpecified()
        {
            CommandLineInterpreterConfiguration.AddCustomConverter(s => s.Length > 1 ? new CustomParamType(s.First(), s.Substring(1)) : null);
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new CustomParamCommand())
                .Description("Description of the whole program.")
                .Positional<CustomParamType>("pos", (command, s) => { })
                    .Description("A positional parameter.")
                .Validator((t, m) => true);

            CommandDescriber.Describe(config, _console, _applicationName, CommandExecutionMode.CommandLine);
            var description = _consoleOutInterface.GetBuffer();
            Console.WriteLine(description);
            Approvals.Verify(description);
        }

        [Fact]
        public void AShortCircuitOptionCanBeSpecified()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new TestCommand())
                .Description("Description of the whole program.")
                .Positional<string>("pos", (command, s) => { })
                    .Description("A positional parameter.")
                .Option("h", (o, b) => { })
                .ShortCircuitOption();
        }

        [Fact]
        public void ARepeatingOptionCanBeSpecified()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new TestCommand())
                .Description("Description of the whole program.")
                .Positional<string>("pos", (command, s) => { })
                    .Description("A positional parameter.")
                .Option("h", (o, b) => { })
                .AllowMultiple();
        }

        [Fact]
        public void ShortCircuitOptionOnAPositionalThrows()
        {
            var config = new CommandLineInterpreterConfiguration();
            void Call() => config
                                .Parameters(() => new TestCommand())
                                .Description("Description of the whole program.")
                                .Positional<string>("pos", (command, s) => { })
                                    .Description("A positional parameter.")
                                    .ShortCircuitOption();

            Assert.Throws<ShortCircuitInvalidOnPositionalParameter>(Call);
        }

        [Fact]
        public void ARepeatingPositionalCanBeSpecified()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new TestCommand())
                .Description("Description of the whole program.")
                .Positional<string>("pos", (command, s) => { })
                    .Description("A positional parameter.")
                    .AllowMultiple();
        }

        [Fact]
        public void AllowMultipleIsInvalidOnCommands()
        {
            var config = new CommandLineInterpreterConfiguration();

            void Call() => config.Command("x", s => new TestCommand())
                    .AllowMultiple();

            Assert.Throws<AllowMultipleInvalid>(Call);
        }

        [Fact]
        public void ACustomParserCanBeSpecifiedOnTheConstructor()
        {
            var config = new CommandLineInterpreterConfiguration(_customParser);
            config.CustomParser.Should().BeSameAs(_customParser);
        }

        [Fact]
        public void ACustomParserSetsTheSelectedConvention()
        {
            var config = new CommandLineInterpreterConfiguration(_customParser);
            Assert.Equal(CommandLineParserConventions.CustomConventions, config.ParserConvention);
        }

        [Fact]
        public void PositionalDefinedOnlyByNameMatchesPropertyAutomatically()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new TestCommand())
                .Positional("IntProp");
            var thePositional = config.DefaultCommand.Positionals[0];
            var thePositionalParameterType = thePositional.GetType().GetGenericArguments()[1];
            Assert.Equal(typeof(int), thePositionalParameterType);
        }

        [Fact]
        public void PositionalDefinedOnlyByNameMatchesIncorrectCasePropertyAutomatically()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new TestCommand())
                .Positional("intprop");
            var thePositional = config.DefaultCommand.Positionals[0];
            var thePositionalParameterType = thePositional.GetType().GetGenericArguments()[1];
            Assert.Equal(typeof(int), thePositionalParameterType);
        }

        [Fact]
        public void PositionalDefinedByNamePrefersCorrectCaseProperty()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new MultiCaseCommand())
                .Positional("AOne");
            var thePositional = config.DefaultCommand.Positionals[0];
            var thePositionalParameterType = thePositional.GetType().GetGenericArguments()[1];
            Assert.Equal(typeof(string), thePositionalParameterType);
        }

        [Fact]
        public void DefaultValueIsInvalidOnAnOption()
        {
            var config = new CommandLineInterpreterConfiguration();

            void Call() => config.Parameters(() => new MultiCaseCommand())
                .Option("AOne")
                .DefaultValue("true");

            Assert.Throws<DefaultValueMayOnlyBeSpecifiedForPositionalParameters>(Call);
        }

        [Fact]
        public void PositionalWithDefaultValueIsOptional()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new MultiCaseCommand())
                .Positional("AOne").DefaultValue("X");
            config.DefaultCommand.Positionals[0].IsOptional.Should().BeTrue();
        }

        [Fact]
        public void DefaultValueIsStoredOnOptionalParameter()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new MultiCaseCommand())
                .Positional("AOne").DefaultValue("X");
            Assert.Equal("X", config.DefaultCommand.Positionals[0].DefaultValue);
        }
    }
}
