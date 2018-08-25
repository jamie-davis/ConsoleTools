using System;
using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestColourSeal
    {
        [Test]
        public void ColourChangeIsPoppedOutAtLineEndAndRestoredAtLineStart()
        {
            var original =
                @"line 1
line 2".Red();

            var testData = ToArray(original);
            var result = MakeResult(ColourSeal.Seal(testData));
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void ForegroundAndBackgroundColoursAreRestored()
        {
            var original =
                @"line 1
line 2".Red().BGCyan() + Environment.NewLine + "line 3 (plain)";

            var testData = ToArray(original);
            var result = MakeResult(ColourSeal.Seal(testData));
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void MultipleColourChangesAreRestoredCorrectly()
        {
            var original = (("Red" + Environment.NewLine + "Lines").Cyan() + Environment.NewLine + "lines").BGDarkRed() + "Clear";

            var testData = ColumnWrapper.WrapValue(original, new ColumnFormat("X"), 20);
            var result = MakeResult(ColourSeal.Seal(testData));
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void NoExtraInstructionsAreGeneratedIfThereIsNoColourInTheText()
        {
            const string original = "Simple plain text with no colour annotations.";

            var testData = ColumnWrapper.WrapValue(original, new ColumnFormat("X"), 20);
            var result = MakeResult(ColourSeal.Seal(testData));
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        private static string MakeResult(string[] data)
        {
            var output = string.Join(Environment.NewLine, data);
            return output
                .Replace(AdapterConfiguration.ControlSequenceIntroducer, '{')
                .Replace(AdapterConfiguration.ControlSequenceTerminator, '}');
        }

        private string[] ToArray(string original)
        {
            var lines = new List<string>();
            var pos = 0;
            var lastPos = 0;
            while ((pos = original.IndexOf(Environment.NewLine, pos, StringComparison.Ordinal)) > 0)
            {
                lines.Add(original.Substring(lastPos, pos - lastPos));
                lastPos = pos + Environment.NewLine.Length;
                pos = lastPos;
            }

            if (lastPos < original.Length)
                lines.Add(original.Substring(lastPos));

            return lines.ToArray();
        }
    }
}