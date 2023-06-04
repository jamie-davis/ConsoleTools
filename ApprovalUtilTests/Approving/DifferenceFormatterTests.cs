using ApprovalUtil.Approving;
using ConsoleToolkit.Testing;
using Approvals = ApprovalTests.Approvals;

namespace ApprovalUtilTests.Approving;

public class DifferenceFormatterTests
{
    [Fact]
    public void DifferencesAreFormatted()
    {
        //Arrange
        var testConsole = new UnitTestConsole(typeof(Program).Namespace);
        var console = testConsole.Console;
        var error = testConsole.Error;

        var received = "The received file";
        var approved = "The approved file";

        //Act
        DifferenceFormatter.DisplayDiffs(received, approved, console, error);

        //Assert
        Approvals.Verify(testConsole.Interface.GetBuffer());
    }
}