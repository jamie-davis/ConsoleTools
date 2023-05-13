using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO
{
    [UseReporter(typeof (CustomReporter))]
    public class TestUnderliner
    {
        private ConsoleInterfaceForTesting _consoleInterface;
        private ConsoleAdapter _adapter;
        public TestUnderliner()
        {
            _consoleInterface = new ConsoleInterfaceForTesting();
            _consoleInterface.BufferWidth = 80;
            _consoleInterface.WindowWidth = 80;

            _adapter = new ConsoleAdapter(_consoleInterface);
        }

        [Fact]
        public void UnderlinerGeneratesDashesByDefault()
        {
            //Arrange
            var heading = "Test heading";
            _adapter.WrapLine(heading);

            //Act
            _adapter.WrapLine(Underliner.Generate(heading));

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void UnderlinerIsCorrectLengthForColouredText()
        {
            //Arrange
            var heading = "Test".Red() + " " + "heading".Blue();
            _adapter.WrapLine(heading);

            //Act
            _adapter.WrapLine(Underliner.Generate(heading));

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void UnderlinerUsesProvidedUnderlineString()
        {
            //Arrange
            var heading = "Test heading";
            _adapter.WrapLine(heading);

            //Act
            _adapter.WrapLine(Underliner.Generate(heading, "*"));

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void UnderlinerCanUseMultiCharacterUnderlineString()
        {
            //Arrange
            var heading = "Test heading";
            _adapter.WrapLine(heading);

            //Act
            _adapter.WrapLine(Underliner.Generate(heading, "*-!^?"));

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Fact]
        public void ColouredTextUnderlinesWork()
        {
            //Arrange
            var heading = "Test".Red() + " " + "heading".Blue();
            _adapter.WrapLine(heading);

            //Act
            _adapter.WrapLine(Underliner.Generate(heading, "*-".White() + "!".Red() + "^?".Green()));

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer(ConsoleBufferFormat.Interleaved));
        }
    }
}