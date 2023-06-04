using ApprovalUtil.Commands;
using ApprovalUtil.Scanning;
using ApprovalUtilTests.TestUtilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using FluentAssertions;
using Approvals = ApprovalTests.Approvals;

namespace ApprovalUtilTests.Commands;

public class CompareCommandTests : IDisposable
{
    private readonly UnitTestConsole _testConsole;
    private readonly IConsoleAdapter _console;
    private readonly IErrorAdapter _error;
    private readonly TestOutputGenerator _testGenerator;
    private readonly List<ApprovalTestResult> _tests;

    public CompareCommandTests()
    {
        _testConsole = new UnitTestConsole(typeof(Program).Namespace);
        _console = _testConsole.Console;
        _error = _testConsole.Error;
        _testGenerator = TestResultObjectMother.GenerateTestsWithFailures(out var results);
        _tests = results;
    }

    [Fact]
    public void CompareCommandWithInvalidFilesShowsError()
    {
        //Arrange
        var compareCommand = new CompareCommand()
        {
            SkipMenu = true,
            Approved = "notapath",
            Received = "notapath"
        };

        //Act
        compareCommand.Handle(_console, _error);

        //Assert
        Approvals.Verify(_testConsole.Interface.GetBuffer());
    }

    [Fact]
    public void CompareCommandWithInvalidFilesSetsExitCode()
    {
        //Arrange
        var compareCommand = new CompareCommand()
        {
            SkipMenu = true,
            Approved = "notapath",
            Received = "notapath"
        };
        Environment.ExitCode = 0;

        //Act
        compareCommand.Handle(_console, _error);

        //Assert
        Environment.ExitCode.Should().Be(-100);
    }

    [Fact]
    public void CompareCommandShowsMenu()
    {
        //Arrange
        var failure = _tests.FirstFailure();
        var compareCommand = new CompareCommand
        {
            SkipMenu = false,
            Approved = failure.Test.ApprovedFile!,
            Received = failure.Test.ReceivedFile!,
        };
        
        var data = @"X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);

        //Act
        compareCommand.Handle(_console, _error);

        //Assert
        Approvals.Verify(_testConsole.Interface.GetBuffer());
    }

    [Fact]
    public void CompareCommandMenuCanBeSkipped()
    {
        //Arrange
        var failure = _tests.FirstFailure();
        var compareCommand = new CompareCommand
        {
            SkipMenu = true,
            Approved = failure.Test.ApprovedFile!,
            Received = failure.Test.ReceivedFile!,
        };

        //Act
        compareCommand.Handle(_console, _error);

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