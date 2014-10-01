using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO.Testing;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Testing
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestUnitTestConsole
    {
        private UnitTestConsole _testInstance;

        [SetUp]
        public void SetUp()
        {
            _testInstance = new UnitTestConsole("testapp");
        }

        [Test]
        public void ConsoleOutputIsWrittenToTestInterface()
        {
            //Act
            _testInstance.Console.WrapLine("Test output.");

            //Assert
            Approvals.Verify(_testInstance.Interface.GetBuffer());
        }

        [Test]
        public void ErrorOutputIsWrittenToTestInterface()
        {
            //Act
            _testInstance.Error.WrapLine("Test error output.");

            //Assert
            Approvals.Verify(_testInstance.Interface.GetBuffer());
        }

        [Test]
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
