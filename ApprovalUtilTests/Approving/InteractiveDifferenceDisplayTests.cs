using ApprovalUtil.Approving;
using ApprovalUtil.Scanning;
using ApprovalUtilTests.TestUtilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using FluentAssertions;
using TestConsoleLib.Testing;

namespace ApprovalUtilTests.Approving;

public class InteractiveDifferenceDisplayTests : IDisposable
{
    private readonly UnitTestConsole _testConsole;
    private readonly IConsoleAdapter _console;
    private readonly IErrorAdapter _error;
    private readonly IDisposable _testGenerator;
    private readonly List<ApprovalTestResult> _tests;

    public InteractiveDifferenceDisplayTests()
    {
        _testConsole = new UnitTestConsole(typeof(Program).Namespace);
        _console = _testConsole.Console;
        _error = _testConsole.Error;
        _testGenerator = TestResultObjectMother.GenerateTestsWithFailures(out var results);
        _tests = results;
    }

    [Fact]
    public void DisplayShowsTestResult()
    {
        //Arrange
        var data = @"X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);
        var failure = _tests.First(t => t.Passed == false);

        //Act
        InteractiveDifferenceDisplay.ShowDifferencesForFailure(_console, _error, failure.Test);

        //Assert
        _testConsole.Interface.GetBuffer().Verify();
    }
    
    [Fact]
    public void TestCanBeApproved()
    {
        //Arrange
        var data = @"A
X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);
        var failure = _tests.First(t => t.Passed == false);

        //Act
        InteractiveDifferenceDisplay.ShowDifferencesForFailure(_console, _error, failure.Test);

        //Assert
        _testConsole.Interface.GetBuffer().Verify();
    }
    
    [Fact]
    public void ReceivedFileIsRemovedForApprovedTest()
    {
        //Arrange
        var data = @"A
X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);
        var test = _tests.First(t => t.Passed == false).Test;

        //Act
        InteractiveDifferenceDisplay.ShowDifferencesForFailure(_console, _error, test);

        //Assert
        File.Exists(test.ReceivedFile).Should().BeFalse();
    }
    
    [Fact]
    public void ApprovedFileIsReplacedForApprovedTest()
    {
        //Arrange
        var data = @"A
X
";
        using var stream = new StringReader(data);
        _testConsole.Interface.SetInputStream(stream);
        var test = _tests.First(t => t.Passed == false && File.Exists(t.Test.ApprovedFile)).Test;
        var receivedText = File.ReadAllText(test.ReceivedFile!);

        //Act
        InteractiveDifferenceDisplay.ShowDifferencesForFailure(_console, _error, test);

        //Assert
        var approvedText = File.ReadAllText(test.ApprovedFile!);
        approvedText.Should().Be(receivedText);
    }

    #region IDisposable

    public void Dispose()
    {
        _testGenerator.Dispose();
    }

    #endregion
}