using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [UseReporter(typeof (CustomReporter))]
    public class TestMicrosoftStandardCommandLineParser
    {
        private MicrosoftStandardCommandLineParser _parser;
        private MockParserResult _result;
        private List<PositionalArg> _positionals;
        private List<OptionDetails> _options;

        class PositionalArg : IPositionalArgument
        {
            public string ParameterName { get; set; }
        }

        class OptionDetails : IOption
        {
            public string Name { get; set; }
            public bool IsBoolean { get; set; }
            public int ParameterCount { get; set; }
        }

        public TestMicrosoftStandardCommandLineParser()
        {
            _positionals = new[] {"pos1", "pos2", "pos3"}.Select(Positional).ToList();
            _options = new[] {"opt", "opt1", "opt2"}.Select(Option).ToList();
            _result = new MockParserResult();
            _parser = new MicrosoftStandardCommandLineParser();
        }

        [Fact]
        public void PositionalParametersAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("parameter1 param2 p3");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void ParsingStopsWhenPositionalReturnsHalt()
        {
            
            var args = CommandLineTokeniser.Tokenise("parameter1 param2 p3");
            _result.HaltAfter(1);
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void NoParameterOptionsAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("parameter1 -opt");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void NamedParametersAreExtractedAsOptions()
        {
            var args = CommandLineTokeniser.Tokenise("-parameter2 parameter1 ");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void NamedParametersAreNotSubjectedToArgSplitting()
        {
            var args = CommandLineTokeniser.Tokenise("-pos2 ,");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void OptionAndParameterNamesAreNotCaseSensitive()
        {
            
            var args = CommandLineTokeniser.Tokenise("-OPt -PoS1 ");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void UnrecognisedOptionsArePassedThroughVerbatim()
        {
            
            var args = CommandLineTokeniser.Tokenise("-OPt -Unrecognized ");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void ConjoinedOptionParametersAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("-Opt1:arg");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void OptionParametersInNextTokenAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("-Opt1 arg");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void MultipleOptionParametersInNextTokenAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("-Opt2 arg,4");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void MultipleConjoinedOptionParametersAreExtracted()
        {
            _options.First(o => o.Name == "opt1").ParameterCount = 3;
            var args = CommandLineTokeniser.Tokenise("-Opt1:arg,45,arg3");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void DoubleDashTokenEndsOptionProcessing()
        {
            
            var args = CommandLineTokeniser.Tokenise("-- -Opt1:arg,45,arg3");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Fact]
        public void ParserIsAlsoAnOptionAdorner()
        {
            _parser.Should().BeAssignableTo<IOptionNameHelpAdorner>();
        }

        [Fact]
        public void OptionNameIsAdorned()
        {
            var adorner = _parser as IOptionNameHelpAdorner;
            Assert.Equal("-X", adorner.Adorn("X"));
        }

        private PositionalArg Positional(string name)
        {
            return new PositionalArg {ParameterName = name};
        }

        private OptionDetails Option(string name)
        {
            var last = name.Last();
            var num = last > '0' && last <= '9' ? last - '0' : 0;

            return new OptionDetails { Name = name, ParameterCount = num};
        }
    }
}
