using ApprovalUtil.Approving;
using ApprovalUtilTests.TestUtilities;
using ConsoleToolkit.Testing;
using FluentAssertions;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace ApprovalUtilTests.Approving;

public class TestOutputLoaderTests
{
    [Fact]
    public void FilesAreLoaded()
    {
        //Arrange
        var testConsole = new UnitTestConsole(typeof(Program).Namespace);
        var error = testConsole.Error;

        using var testData = TestResultObjectMother.GenerateTestsWithFailures(out var scanResult);
        var failedTestResult = scanResult.First(t => !t.Passed 
                                                     && File.Exists(t.Test.ReceivedFile) 
                                                     && File.Exists(t.Test.ApprovedFile)).Test;
        
        //Act
        var (received, approved) = TestOutputLoader.LoadText(failedTestResult, error);
        
        //Assert
        var output = new Output();
        output.WrapLine("Received:");
        output.WrapLine(received);
        output.WriteLine();
        output.WriteLine();
        output.WrapLine("Approved:");
        output.WrapLine(approved);
        output.Report.Verify();

    }

    [Fact]
    public void MissingReceivedFileIsBlank()
    {
        //Arrange
        var testConsole = new UnitTestConsole(typeof(Program).Namespace);
        var error = testConsole.Error;

        using var testData = TestResultObjectMother.GenerateTestsWithFailures(out var scanResult);
        var failedTestResult = scanResult.First(t => !File.Exists(t.Test.ReceivedFile) 
                                                     && File.Exists(t.Test.ApprovedFile)).Test;
        
        //Act
        var (received, _) = TestOutputLoader.LoadText(failedTestResult, error);
        
        //Assert
        received.Should().BeEmpty();
    }

    [Fact]
    public void MissingApprovedFileIsBlank()
    {
        //Arrange
        var testConsole = new UnitTestConsole(typeof(Program).Namespace);
        testConsole.Interface.WindowWidth = 1000;
        testConsole.Interface.BufferWidth = 1000;
        var error = testConsole.Error;

        using var testData = TestResultObjectMother.GenerateTestsWithFailures(out var scanResult);
        var failedTestResult = scanResult.First(t => File.Exists(t.Test.ReceivedFile) 
                                                     && !File.Exists(t.Test.ApprovedFile)).Test;
        
        //Act
        var (_, approved) = TestOutputLoader.LoadText(failedTestResult, error);
        
        //Assert
        approved.Should().BeEmpty();
    }
    
    [Fact]
    public void InaccessibleApprovedFileIsReportedAsError()
    {
        //Arrange
        var testConsole = new UnitTestConsole(typeof(Program).Namespace);
        var error = testConsole.Error;

        using var testData = TestResultObjectMother.GenerateTestsWithFailures(out var scanResult);
        var testResult = scanResult.First(t => File.Exists(t.Test.ApprovedFile)).Test;
        
        //Act
        using (var _ = File.OpenWrite(testResult.ApprovedFile!))
            TestOutputLoader.LoadText(testResult, error);
        
        //Assert
        testConsole.Interface.GetBuffer().Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void InaccessibleReceivedFileIsReportedAsError()
    {
        //Arrange
        var testConsole = new UnitTestConsole(typeof(Program).Namespace);
        var error = testConsole.Error;

        using var testData = TestResultObjectMother.GenerateTestsWithFailures(out var scanResult);
        var testResult = scanResult.First(t => File.Exists(t.Test.ReceivedFile)).Test;
        
        //Act
        using (var _ = File.OpenWrite(testResult.ReceivedFile!))
            TestOutputLoader.LoadText(testResult, error);
        
        //Assert
        testConsole.Interface.GetBuffer().Should().NotBeNullOrWhiteSpace();
    }
}