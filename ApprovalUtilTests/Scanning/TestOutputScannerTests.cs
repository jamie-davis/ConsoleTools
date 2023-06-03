using ApprovalUtil.Scanning;
using ApprovalUtilTests.Approving;
using ApprovalUtilTests.TestUtilities;
using FluentAssertions;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace ApprovalUtilTests.Scanning;

public class TestOutputScannerTests
{
    [Fact]
    public void UnitTestOutputsAreExtractedForThisApplication()
    {
        //Arrange
        using var testData = TestResultObjectMother.GenerateTestsWithFailures();

        //Act
        var result = TestOutputScanner.Scan(testData.FolderPath);

        //Assert
        var output = new Output();
        var formattedResult = result
            .Select(a => new
            {
                a.TestTypeName,
                a.TestName,
                Approved = Path.GetFileName(a.ApprovedFile), 
                Received = Path.GetFileName(a.ReceivedFile), 
                Source = Path.GetFileName(a.SourceFile),
            })
            .OrderBy(a => a.TestTypeName)
            .ThenBy(a => a.TestName);
        output.FormatTable(formattedResult, ReportFormattingOptions.UnlimitedBuffer);
        Approvals.Verify(output.Report);
    }
    
    [Fact]
    public void UnitTestOutputIsDerivedFromTextFiles()
    {
        //Arrange
        using var testData = TestResultObjectMother.GenerateTestsWithFailures();
        var tests = TestOutputScanner.Scan(testData.FolderPath);
        var failure = tests.FirstFailure(); 

        //Act
        var result = TestOutputScanner.OutputFromResultFiles(failure.ReceivedFile, failure.ApprovedFile);

        //Assert
        result.TestName.Should().NotBeEmpty();
    }
}
