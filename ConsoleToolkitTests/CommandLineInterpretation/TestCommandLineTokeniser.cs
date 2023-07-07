using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandLineTokeniser
    {
        [Fact]
        public void SimpleArgsAreExtracted()
        {
            Approvals.Verify(Tokenise("simple args are space delimited"));
        }

        [Fact]
        public void ArgsWithStringDelimiterAreExtracted()
        {
            Approvals.Verify(Tokenise("simple \"this one is delimited\""));
        }

        [Fact]
        public void EscapedStringDelimitersArePartOfTheToken()
        {
            Approvals.Verify(Tokenise("simple \"containing \\\"embedded speech\\\"\""));
        }

        [Fact]
        public void ForcedStringDelimitersArePartOfTheToken()
        {
            Approvals.Verify(Tokenise("simple \"containing \"\"embedded speech\"\"\""));
        }

        [Fact]
        public void CommaSeperatedValuesAreTheSameToken()
        {
            Approvals.Verify(Tokenise("a b c,d,e"));
        }

        [Fact]
        public void SpeechMarksAreRemovedFromCsvText()
        {
            Approvals.Verify(Tokenise("a b c,\"text\""));
        }

        [Fact]
        public void EmbeddedSpacesRemainInSpeechMarkTextInCsvToken()
        {
            Approvals.Verify(Tokenise("a b c,\"text with space\""));
        }

        [Fact]
        public void CommaInEmbeddedSpeechMarkDelimitedCsvTextIsAProblem()
        {
            Approvals.Verify(Tokenise("a b c,\"text with space, and a comma\""));
        }

        [Fact]
        public void EmbeddedSpeechMarksAreUsedToExtendTheTokenButDropped()
        {
            Approvals.Verify(Tokenise("a b c\"text with a space, and a comma\""));
        }

        [Fact]
        public void EmbeddedSpeechMarksInMultipleArgsBehaveCorrectly()
        {
            Approvals.Verify(Tokenise("a b c\"text with a space, and a comma\" e\"more text with a space, and a comma\""));
        }

        [Fact]
        public void EmptyTokensAreExtracted()
        {
            Approvals.Verify(Tokenise("\"\""));
        }

        private string Tokenise(string commandLine)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Tokens extracted from:");
            sb.AppendFormat("-->{0}<--", commandLine);
            sb.AppendLine();
            sb.AppendLine();

            var args = CommandLineTokeniser.Tokenise(commandLine);

            var count = 0;
            foreach (var arg in args)
            {
                sb.AppendFormat("{0}: [{1}]", count++, arg);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
