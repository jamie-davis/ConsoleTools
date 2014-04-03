using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandLineInterpreterConfiguration
    {
        private CommandLineInterpreterConfiguration _config;
        private CustomParser _customParser;

        public class CustomParser : ICommandLineParser
        {
            public void Parse(string[] args, IEnumerable<IOption> options, IEnumerable<IPositionalArgument> positionalArguments, IParserResult result)
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

        [SetUp]
        public void SetUp()
        {
            _config = new CommandLineInterpreterConfiguration();
            _config
                .Command("first", s => new TestCommand())
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
                .Option("on", (command, b) => {})
                    .Description("A simple option with no argument.")
                .Option<string, int>("fiddly", (command, s, n) => {})
                    .Alias("f")
                    .Description("An option with two arguments. The arguments need to be described in the text.");
            _config
                .Command("fourth", s => new TestCommand())
                .Description("The fourth command is really complicated with a number of parameters and also options. This is the sort of command that needs lots of text.")
                .Positional<string>("date", (command, s) => { })
                    .Description("The date the complicated nonsense should be forgotten.")
                .Positional<string>("crpyticnum", (command, s) => { })
                    .Description("The amount of nonsense the user needs to forget.")
                .Option("ignore", (command, b) => {})
                    .Description("Use this option to consign this command to history, where it belongs.")
                .Option<string, int>("more", (command, s, n) => {})
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
                .Option("B", command => command.BoolProp )
                    .Description("A boolean option configured with an expression.")
                .Option("I", command => command.IntProp )
                    .Description("A boolean option configured with an expression.");
        }

        [Test, ExpectedException(typeof(InvalidParameterType))]
        public void InvalidOptionParameterTypeThrows()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<XDocument>("opt", (s, x) => { });
        }

        [Test]
        public void OptionsCanHaveAliases()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { })
                .Alias("optalias");
        }

        [Test, ExpectedException(typeof(DuplicateOptionName))]
        public void OptionsAliasSameAsOptionNameThrows()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { })
                .Alias("opt");
        }

        [Test, ExpectedException(typeof(DuplicateOptionName))]
        public void DuplicateOptionAliasThrows()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { })
                .Alias("o")
                .Alias("o");
        }

        [Test, ExpectedException(typeof(DuplicateOptionName))]
        public void AliasSameAsOtherOptionNameThrows()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { })
                .Option<string>("opt2", (s, x) => { })
                .Alias("opt");
        }

        [Test, ExpectedException(typeof(DuplicateOptionName))]
        public void AliasSameAsOtherOptionAliasThrows()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { })
                .Alias("alias")
                .Option<string>("opt2", (s, x) => { })
                .Alias("alias");
        }

        [Test, ExpectedException(typeof (AliasNotSupported))]
        public void CommandAliasThrows()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Alias("invalid")
                .Option<string>("opt", (s, x) => { });
        }

        [Test, ExpectedException(typeof(AliasNotSupported))]
        public void PositionalAliasThrows()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Command("test", s => s)
                .Positional<string>("pos", (c, b) => {})
                .Alias("invalid")
                .Option<string>("opt", (s, x) => { });
        }

        [Test, ExpectedException(typeof(CommandAlreadySpecified))]
        public void ConfiguringTheSameCommandTwiceThrows()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { });
            config
                .Command("test", s => s)
                .Option<string>("opt", (s, x) => { });
        }

        [Test, ExpectedException(typeof(InvalidParameterType))]
        public void InvalidCommandParameterTypeThrows()
        {
            var config = new CommandLineInterpreterConfiguration()
                .Parameters(() => "test")
                .Positional<XDocument>("opt", (s, x) => { });
        }

        [Test, ExpectedException(typeof(ProgramParametersAlreadySpecified))]
        public void ConfiguringDefaultCommandTwiceThrows()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => "test")
                .Positional<string>("opt", (s, x) => { });
            config
                .Parameters(() => "test again")
                .Positional<int>("opt2", (s, x) => { });
        }

        [Test]
        public void DescriptionOfCommandsIsFormatted()
        {
            var fortyWide = "1234567890123456789012345678901234567890" + Environment.NewLine;
            Approvals.Verify(fortyWide + _config.Describe(40));
        }

        [Test]
        public void DefaultCommandHelpIsFormatted()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new TestCommand())
                .Description("Description of the whole program.")
                .Positional<string>("pos", (command, s) => { })
                    .Description("A positional parameter.");

            var fortyWide = "1234567890123456789012345678901234567890" + Environment.NewLine;
            Approvals.Verify(fortyWide + config.Describe(40));
        }

        [Test]
        public void ACustomConverterCanBeSpecifiedAndUsed()
        {
            CommandLineInterpreterConfiguration.AddCustomConverter(s => s.Length > 1 ? new CustomParamType(s.First(), s.Substring(1)) : null);
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new CustomParamCommand())
                .Description("Description of the whole program.")
                .Positional<CustomParamType>("pos", (command, s) => { })
                    .Description("A positional parameter.");

            var fortyWide = "1234567890123456789012345678901234567890" + Environment.NewLine;
            Approvals.Verify(fortyWide + config.Describe(40));
        }

        [Test]
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

            var fortyWide = "1234567890123456789012345678901234567890" + Environment.NewLine;
            Approvals.Verify(fortyWide + config.Describe(40));
        }

        [Test]
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

        [Test, ExpectedException(typeof (ShortCircuitInvalidOnPositionalParameter))]
        public void ShortCircuitOptionOnAPositionalThrows()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new TestCommand())
                .Description("Description of the whole program.")
                .Positional<string>("pos", (command, s) => { })
                    .Description("A positional parameter.")
                    .ShortCircuitOption();
        }

        [Test]
        public void ACustomParserCanBeSpecifiedOnTheConstructor()
        {
            var config = new CommandLineInterpreterConfiguration(_customParser);
            Assert.That(config.CustomParser, Is.SameAs(_customParser));
        }

        [Test]
        public void ACustomParserSetsTheSelectedConvention()
        {
            var config = new CommandLineInterpreterConfiguration(_customParser);
            Assert.That(config.ParserConvention, Is.EqualTo(CommandLineParserConventions.CustomConventions));
        }
    }
}
