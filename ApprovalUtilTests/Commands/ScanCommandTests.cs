using ApprovalUtil.Commands;
using ApprovalUtil.Scanning;
using ApprovalUtilTests.TestUtilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using FluentAssertions;
using TestConsoleLib.Testing;

namespace ApprovalUtilTests.Commands;

public class ScanCommandTests : IDisposable
{
    private readonly UnitTestConsole _testConsole;
    private readonly IConsoleAdapter _console;
    private readonly IErrorAdapter _error;
    private readonly TestOutputGenerator _testGenerator;
    private readonly List<ApprovalTestResult> _tests;

    public ScanCommandTests()
    {
        _testConsole = new UnitTestConsole(typeof(Program).Namespace);
        _console = _testConsole.Console;
        _error = _testConsole.Error;
        _testGenerator = TestResultObjectMother.GenerateTestsWithFailures(out var results);
        _tests = results;
    }

    [Fact]
    public void ScanCommandWithInvalidFolderShowsAnError()
    {
        //Arrange
        var scanCommand = new ScanCommand
        {
            Interactive = false,
            PathToCode = "notapath"
        };

        //Act
        scanCommand.Handle(_console, _error);

        //Assert
        Approvals.Verify(_testConsole.Interface.GetBuffer());
    }

    [Fact]
    public void ScanCommandWithInvalidFolderSetsExitCode()
    {
        //Arrange
        var scanCommand = new ScanCommand
        {
            Interactive = false,
            PathToCode = "notapath"
        };
        Environment.ExitCode = 0;

        //Act
        scanCommand.Handle(_console, _error);

        //Assert
        Environment.ExitCode.Should().Be(-100);
    }

    [Fact]
    public void ScanCommandShowsMenu()
    {
        //Arrange
        var scanCommand = new ScanCommand
        {
            Interactive = true,
            PathToCode = _testGenerator.FolderPath
        };
        
        var data = @"X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);

        //Act
        scanCommand.Handle(_console, _error);

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