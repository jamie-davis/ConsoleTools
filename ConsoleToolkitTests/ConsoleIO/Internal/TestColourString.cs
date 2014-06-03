using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    public class TestColourString
    {
        const string SimpleString = "normal data with no colour";
        static readonly string ColouredString = "annotated".Red() + " data".Yellow() + " with".Green() + " colour".BGCyan();
        static readonly string Control = "annotated" + " data" + " with" + " colour"; //same as coloured string, but without colour

        [Test]
        public void SimpleStringIsMeasured()
        {
            Assert.That(ColourString.Length(SimpleString), Is.EqualTo(SimpleString.Length));
        }

        [Test]
        public void ColouredStringIsMeasured()
        {
            Assert.That(ColourString.Length(ColouredString), Is.EqualTo(Control.Length));
        }

        [Test]
        public void CompletelyEmptyStringHasZeroLength()
        {
            Assert.That(ColourString.Length(string.Empty), Is.EqualTo(0));
        }

        [Test]
        public void EmptyStringWithColoursHasZeroLength()
        {
            Assert.That(ColourString.Length(string.Empty.Red().BGBlack()), Is.EqualTo(0));
        }

        [Test]
        public void SubstringReturnsStringBeginning()
        {
            Assert.That(ColourString.Substring(SimpleString, 0, 5), Is.EqualTo(SimpleString.Substring(0, 5)));
        }

        [Test]
        public void SubtringReturnsColouredStringBeginning()
        {
            Assert.That(ColourString.Substring(ColouredString, 0, 5), Is.EqualTo("annot".Red() + "".Yellow() + "".Green() + "".BGCyan()));
        }

        [Test]
        public void SubstringReturnsSectionFromString()
        {
            Assert.That(ColourString.Substring(SimpleString, 5, 3), Is.EqualTo(SimpleString.Substring(5, 3)));
        }

        [Test]
        public void SubstringReturnsSectionFromColouredString()
        {
            Assert.That(ColourString.Substring(ColouredString, 8, 3), Is.EqualTo("d".Red()+" d".Yellow()+"".Green()+"".BGCyan()));
        }

        [Test]
        public void SubstringReturnsStringEnd()
        {
            Assert.That(ColourString.Substring(SimpleString, 20), Is.EqualTo(SimpleString.Substring(20)));
        }

        [Test]
        public void SubstringReturnsColouredStringEnd()
        {
            Assert.That(ColourString.Substring(ColouredString, 22), Is.EqualTo("".Red() + "".Yellow() + "".Green() + "lour".BGCyan()));
        }
    }
}