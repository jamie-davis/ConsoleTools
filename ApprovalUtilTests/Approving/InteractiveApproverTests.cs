using ApprovalUtil.Approving;
using ApprovalUtil.Scanning;
using ApprovalUtilTests.TestUtilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using TestConsoleLib.Testing;

namespace ApprovalUtilTests.Approving;

public class InteractiveApproverTests : IDisposable
{
    private readonly UnitTestConsole _testConsole;
    private readonly IConsoleAdapter _console;
    private readonly IErrorAdapter _error;
    private readonly IDisposable _testGenerator;
    private readonly List<ApprovalTestResult> _tests;

    public InteractiveApproverTests()
    {
        _testConsole = new UnitTestConsole(typeof(Program).Namespace);
        _console = _testConsole.Console;
        _error = _testConsole.Error;
        _testGenerator = TestResultObjectMother.GenerateTestsWithFailures(out var results);
        _tests = results;
    }

    [Fact]
    public void InteractiveApproverDescribesResultsAndShowsMenu()
    {
        //Arrange
        var data = @"X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);

        //Act
        var _ = InteractiveApprover.Execute(_console, _error, _tests);

        //Assert
        Approvals.Verify(_testConsole.Interface.GetBuffer());
    }
    
    [Fact]
    public void InteractiveApproverShowsTestResults()
    {
        //Arrange
        var data = @"1
X
X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);

        //Act
        var _ = InteractiveApprover.Execute(_console, _error, _tests);

        //Assert
        Approvals.Verify(_testConsole.Interface.GetBuffer());
    }

    [Fact]
    public void ApprovedTestIsRemovedFromMenu()
    {
        //Arrange
        var data = @"1
A
X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);

        //Act
        var _ = InteractiveApprover.Execute(_console, _error, _tests);

        //Assert
        Approvals.Verify(_testConsole.Interface.GetBuffer());
    }

    #region IDisposable

    public void Dispose()
    {
        _testGenerator.Dispose();
    }

    #endregion
}