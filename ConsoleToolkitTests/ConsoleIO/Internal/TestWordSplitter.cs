using System;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestWordSplitter
    {
        [Test]
        public void WordsAreExractedWithTrailingSpaceCounts()
        {
            const string testPhrase = "one two  three   none.";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = words
                .Select(wo => string.Format(@"[{0},{1},""{2}""]", wo.Length, wo.TrailingSpaces, wo.WordValue))
                .Aggregate((t, i) => t + i);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]"));
        }

        [Test]
        public void TabsAreConvertedIntoSpaces()
        {
            const string testPhrase = "one\ttwo\t three \t eight.\t\t";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = words
                .Select(wo => string.Format(@"[{0},{1},""{2}""]", wo.Length, wo.TrailingSpaces, wo.WordValue))
                .Aggregate((t, i) => t + i);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,4,""one""][3,5,""two""][5,6,""three""][6,8,""eight.""]"));
        }

        [Test]
        public void NewlinesCreateTerminatesLineWord()
        {
            const string testPhrase = "one\ttwo\t \nthree \t eight.\t\t\r";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = words
                .Select(wo => string.Format("[{0},{1},T:{2}]", wo.Length, wo.TrailingSpaces, wo.TerminatesLine ? "yes" : "no"))
                .Aggregate((t, i) => t + i);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,4,T:no][3,5,T:yes][5,6,T:no][6,8,T:yes]"));
        }

        [Test]
        public void CrLfCountsAsOneLineEnd()
        {
            const string testPhrase = "one\r\ntwo\r\n\r\nthree\n\n\r\r";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = words
                .Select(wo => string.Format("[{0},{1},T:{2}]", wo.Length, wo.TrailingSpaces, wo.TerminatesLine ? "yes" : "no"))
                .Aggregate((t, i) => t + i);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo("[3,0,T:yes][3,0,T:yes][0,0,T:yes][5,0,T:yes][0,0,T:yes][0,0,T:yes][0,0,T:yes]"));
        }
    }
}