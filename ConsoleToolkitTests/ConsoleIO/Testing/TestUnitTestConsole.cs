using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Testing
{
    [UseReporter(typeof(CustomReporter))]
    public class TestUnitTestConsole
    {
        private UnitTestConsole _testInstance;
        public TestUnitTestConsole()
        {
            _testInstance = new UnitTestConsole("testapp");
        }

        [Fact]
        public void ConsoleOutputIsWrittenToTestInterface()
        {
            //Act
            _testInstance.Console.WrapLine("Test output.");

            //Assert
            Approvals.Verify(_testInstance.Interface.GetBuffer());
        }

        [Fact]
        public void ErrorOutputIsWrittenToTestInterface()
        {
            //Act
            _testInstance.Error.WrapLine("Test error output.");

            //Assert
            Approvals.Verify(_testInstance.Interface.GetBuffer());
        }

        [Fact]
        public void AllOutputIsWrittenToTestInterface()
        {
            //Act
            _testInstance.Console.WrapLine("Test output.");
            _testInstance.Error.WrapLine("Test error output.");

            //Assert
            Approvals.Verify(_testInstance.Interface.GetBuffer());
        }
    }
}
