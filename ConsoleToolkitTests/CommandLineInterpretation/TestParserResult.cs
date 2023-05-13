using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [UseReporter(typeof (CustomReporter))]
    public class TestParserResult
    {
        private ParserResult _parserResult;
        private BaseCommandConfig _command;
        private CommandLineInterpreterConfiguration _config;

        class CommandParams
        {
            public string Pos1 { get; set; }
            public int Pos2 { get; set; }
            public string Opt1String { get; set; }
            public int Opt1Int { get; set; }
            public bool Opt2 { get; set; }
            public string Opt3 { get; set; }

            public string Opt1Settings()
            {
                return string.Format("\"{0}\",{1}", Opt1String, Opt1Int);
            }
        }
        public TestParserResult()
        {
            _config = new CommandLineInterpreterConfiguration();
            _command = _config.Parameters(() => new CommandParams())
                .Positional<string>("pos1", (c, s) => c.Pos1 = s)
                .Positional<int>("pos2", (c, i) => c.Pos2 = i)
                .Option<string, int>("opt1", (c, s, i) =>
                {
                    c.Opt1String = s;
                    c.Opt1Int = i;
                })
                    .Alias("1")
                    .Alias("one")
                .Option("opt2", (c, b) => c.Opt2 = b)
                    .Alias("2")
                    .Alias("two")
                .Option<string>("opt3", (c, s) => c.Opt3 = s)
                    .Alias("3")
                    .Alias("three");
            _parserResult = new ParserResult(_command, "commandName", null, CommandExecutionMode.CommandLine);
        }

        [Fact]
        public void PositionalParametersAreApplied()
        {
            _parserResult.PositionalArgument("pos");
            var paramObject = (CommandParams)_parserResult.ParamObject;
            Assert.Equal("pos", paramObject.Pos1);
        }

        [Fact]
        public void ExpectedPositionalParametersCauseParserContinue()
        {
            var result = _parserResult.PositionalArgument("pos");
            Assert.Equal(ParseOutcome.Continue, result);
        }

        [Fact]
        public void CommandObjectIsInstantiated()
        {
            _parserResult.ParamObject.Should().BeOfType<CommandParams>();
        }

        [Fact]
        public void UnexpectedPositionalsGenerateError()
        {
            _parserResult.PositionalArgument("pos");
            _parserResult.PositionalArgument("12");
            _parserResult.PositionalArgument("invalid");
            Assert.Equal("Unexpected argument \"invalid\"", _parserResult.Error);
        }

        [Fact]
        public void UnexpectedPositionalsCauseParsingHalt()
        {
            _parserResult.PositionalArgument("pos");
            _parserResult.PositionalArgument("12");
            var result = _parserResult.PositionalArgument("invalid");
            Assert.Equal(ParseOutcome.Halt, result);
        }

        [Fact]
        public void InvalidValueForPositionalGeneratesError()
        {
            _parserResult.PositionalArgument("pos");
            _parserResult.PositionalArgument("invalid");
            Assert.Equal("The pos2 parameter value \"invalid\" is invalid.", _parserResult.Error);
        }

        [Fact]
        public void InvalidValueForPositionalCauseParsingHalt()
        {
            _parserResult.PositionalArgument("pos");
            var result = _parserResult.PositionalArgument("invalid");
            Assert.Equal(ParseOutcome.Halt, result);
        }

        [Fact]
        public void OptionsAreApplied()
        {
            _parserResult.OptionExtracted("opt1", new []{"str", "10"});
            var paramObject = (CommandParams)_parserResult.ParamObject;
            Assert.Equal("\"str\",10", paramObject.Opt1Settings());
        }

        [Fact]
        public void OptionsAreAppliedViaAlias()
        {
            _parserResult.OptionExtracted("1", new []{"str", "10"});
            var paramObject = (CommandParams)_parserResult.ParamObject;
            Assert.Equal("\"str\",10", paramObject.Opt1Settings());
        }

        [Fact]
        public void ExpectedOptionsCauseParsingContinue()
        {
            var result = _parserResult.OptionExtracted("opt1", new []{"str", "10"});
            Assert.Equal(ParseOutcome.Continue, result);
        }

        [Fact]
        public void UnexpectedOptionsCauseParsingHalt()
        {
            var result = _parserResult.OptionExtracted("invalid", new []{"str", "10"});
            Assert.Equal(ParseOutcome.Halt, result);
        }

        [Fact]
        public void UnexpectedOptionsGenerateAnError()
        {
            var result = _parserResult.OptionExtracted("invalid", new []{"str", "10"});
            Assert.Equal("\"invalid\" is not a valid option.", _parserResult.Error);
        }

        [Fact]
        public void InvalidOptionArgumentCausesParsingHalt()
        {
            var result = _parserResult.OptionExtracted("opt1", new[] { "str", "non-numeric" });
            Assert.Equal(ParseOutcome.Halt, result);
        }

        [Fact]
        public void InvalidOptionArgumentGeneratesAnError()
        {
            var result = _parserResult.OptionExtracted("opt1", new[] { "str", "non-numeric" });
            Assert.Equal("The parameter \"non-numeric\" of the opt1 option has an invalid value.", _parserResult.Error);
        }

        [Fact]
        public void ExcessiveOptionArgumentGeneratesAnError()
        {
            var result = _parserResult.OptionExtracted("opt1", new[] { "str", "5", "toomany" });
            Assert.Equal("The opt1 option has too many parameters.", _parserResult.Error);
        }

        [Fact]
        public void InsufficientOptionArgumentGeneratesAnError()
        {
            var result = _parserResult.OptionExtracted("opt1", new[] { "str" });
            Assert.Equal("Not enough parameters for the opt1 option.", _parserResult.Error);
        }

        [Fact]
        public void OptionsGenerateAnErrorIfUsedMoreThanOnce()
        {
            _parserResult.OptionExtracted("opt1", new[] { "str", "10" });
            var result = _parserResult.OptionExtracted("opt1", new[] { "str", "11" });
            Assert.Equal("The \"opt1\" option may only be specified once.", _parserResult.Error);
        }

        [Fact]
        public void OptionsGenerateAnErrorIfUsedMoreThanOnceViaAlias()
        {
            _parserResult.OptionExtracted("opt1", new[] { "str", "10" });
            var result = _parserResult.OptionExtracted("one", new[] { "str", "11" });
            Assert.Equal("The \"opt1\" option may only be specified once.", _parserResult.Error);
        }

        [Fact]
        public void OptionsCauseParsingHaltIfUsedMoreThanOnce()
        {
            _parserResult.OptionExtracted("opt1", new[] { "str", "10" });
            var result = _parserResult.OptionExtracted("opt1", new[] { "str", "11" });
            Assert.Equal(ParseOutcome.Halt, result);
        }

        [Fact]
        public void PositionalsPresentedAsOptionsAreNotProcessedAsPositionals()
        {
            _parserResult.OptionExtracted("pos1", new[] { "str", "10" });
            _parserResult.PositionalArgument("7");
            var paramObject = (CommandParams)_parserResult.ParamObject;
            var result = string.Format("Pos1 = \"{0}\", Pos2 = {1}", paramObject.Pos1, paramObject.Pos2);
            Assert.Equal("Pos1 = \"str\", Pos2 = 7", result);
        }

        [Fact]
        public void ItIsNotAnErrorForAPositionalToBePresentedAsAnOption()
        {
            var result = _parserResult.OptionExtracted("pos1", new[] { "str", "10" });
            Assert.Equal(ParseOutcome.Continue, result);
        }

        [Fact]
        public void UsedPositionalsPresentedAsOptionsGenerateAnError()
        {
            _parserResult.PositionalArgument("pos");
            var result = _parserResult.OptionExtracted("pos1", new[] { "str" });
            Assert.Equal("The \"pos1\" parameter may only be specified once.", _parserResult.Error);
        }

        [Fact]
        public void UsedPositionalsPresentedAsOptionsCauseParserHalt()
        {
            _parserResult.PositionalArgument("pos");
            var result = _parserResult.OptionExtracted("pos1", new[] { "str", "10" });
            Assert.Equal(ParseOutcome.Halt, result);
        }

        [Fact]
        public void ParsingCompleteShouldPerformErrorCheck()
        {
            _parserResult.PositionalArgument("str");
            _parserResult.PositionalArgument("10");
            _parserResult.ParseCompleted();
            Assert.Equal(ParseStatus.CompletedOk, _parserResult.Status);
        }

        [Fact]
        public void ParsingCompleteShouldSetErrorStatusAfterInvalidInput()
        {
            _parserResult.PositionalArgument("str");
            _parserResult.PositionalArgument("not valid");
            _parserResult.ParseCompleted();
            Assert.Equal(ParseStatus.Failed, _parserResult.Status);
        }

        [Fact]
        public void ParsingCompleteShouldSetErrorStatusAfterInvalidOption()
        {
            _parserResult.PositionalArgument("str");
            _parserResult.PositionalArgument("10");
            _parserResult.OptionExtracted("invalid", new string[] {});
            _parserResult.ParseCompleted();
            Assert.Equal(ParseStatus.Failed, _parserResult.Status);
        }

        [Fact]
        public void ParsingCompleteShouldSetErrorStatusIfNotAllPositionalsPresent()
        {
            _parserResult.PositionalArgument("str");
            _parserResult.ParseCompleted();
            Assert.Equal(ParseStatus.Failed, _parserResult.Status);
        }

        [Fact]
        public void ParsingCompleteShouldSetErrorMessageIfNotAllPositionalsPresent()
        {
            _parserResult.PositionalArgument("str");
            _parserResult.ParseCompleted();
            Assert.Equal("Not enough parameters specified.", _parserResult.Error);
        }
    }
}
