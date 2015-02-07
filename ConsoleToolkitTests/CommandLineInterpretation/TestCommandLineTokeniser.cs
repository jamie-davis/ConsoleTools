using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandLineTokeniser
    {
        [Test]
        public void SimpleArgsAreExtracted()
        {
            Approvals.Verify(Tokenise("simple args are space delimited"));
        }

        [Test]
        public void ArgsWithStringDelimiterAreExtracted()
        {
            Approvals.Verify(Tokenise("simple \"this one is delimited\""));
        }

        [Test]
        public void EscapedStringDelimitersArePartOfTheToken()
        {
            Approvals.Verify(Tokenise("simple \"containing \\\"embedded speech\\\"\""));
        }

        [Test]
        public void ForcedStringDelimitersArePartOfTheToken()
        {
            Approvals.Verify(Tokenise("simple \"containing \"\"embedded speech\"\"\""));
        }

        [Test]
        public void CommaSeperatedValuesAreTheSameToken()
        {
            Approvals.Verify(Tokenise("a b c,d,e"));
        }

        [Test]
        public void SpeechMarksAreRemovedFromCsvText()
        {
            Approvals.Verify(Tokenise("a b c,\"text\""));
        }

        [Test]
        public void EmbeddedSpacesRemainInSpeechMarkTextInCsvToken()
        {
            Approvals.Verify(Tokenise("a b c,\"text with space\""));
        }

        [Test]
        public void CommaInEmbeddedSpeechMarkDelimitedCsvTextIsAProblem()
        {
            Approvals.Verify(Tokenise("a b c,\"text with space, and a comma\""));
        }

        [Test]
        public void EmbeddedSpeechMarksAreUsedToExtendTheTokenButDropped()
        {
            Approvals.Verify(Tokenise("a b c\"text with a space, and a comma\""));
        }

        [Test]
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
