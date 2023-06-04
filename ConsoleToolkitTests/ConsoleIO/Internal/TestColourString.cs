using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    public class TestColourString
    {
        const string SimpleString = "normal data with no colour";
        static readonly string ColouredString = "annotated".Red() + " data".Yellow() + " with".Green() + " colour".BGCyan();
        static readonly string Control = "annotated" + " data" + " with" + " colour"; //same as coloured string, but without colour

        [Fact]
        public void SimpleStringIsMeasured()
        {
            Assert.Equal(SimpleString.Length, ColourString.Length(SimpleString));
        }

        [Fact]
        public void ColouredStringIsMeasured()
        {
            Assert.Equal(Control.Length, ColourString.Length(ColouredString));
        }

        [Fact]
        public void CompletelyEmptyStringHasZeroLength()
        {
            Assert.Equal(0, ColourString.Length(string.Empty));
        }

        [Fact]
        public void EmptyStringWithColoursHasZeroLength()
        {
            Assert.Equal(0, ColourString.Length(string.Empty.Red().BGBlack()));
        }

        [Fact]
        public void SubstringReturnsStringBeginning()
        {
            Assert.Equal(SimpleString.Substring(0, 5), ColourString.Substring(SimpleString, 0, 5));
        }

        [Fact]
        public void SubtringReturnsColouredStringBeginning()
        {
            Assert.Equal("annot".Red() + "".Yellow() + "".Green() + "".BGCyan(), ColourString.Substring(ColouredString, 0, 5));
        }

        [Fact]
        public void SubstringReturnsSectionFromString()
        {
            Assert.Equal(SimpleString.Substring(5, 3), ColourString.Substring(SimpleString, 5, 3));
        }

        [Fact]
        public void SubstringReturnsSectionFromColouredString()
        {
            Assert.Equal("d".Red() + " d".Yellow() + "".Green() + "".BGCyan(), ColourString.Substring(ColouredString, 8, 3));
        }

        [Fact]
        public void SubstringReturnsStringEnd()
        {
            Assert.Equal(SimpleString.Substring(20), ColourString.Substring(SimpleString, 20));
        }

        [Fact]
        public void SubstringReturnsColouredStringEnd()
        {
            Assert.Equal("".Red() + "".Yellow() + "".Green() + "lour".BGCyan(), ColourString.Substring(ColouredString, 22));
        }
    }
}