using ApprovalUtil.Scanning;
using ApprovalUtilTests.TestUtilities;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace ApprovalUtilTests.Scanning;

public class TestResultScannerTests
{
    [Fact]
    public void ResultsAreExtracted()
    {
        //Arrange
        using var testData = new TestOutputGenerator();
        testData.MakeTest("TestClass1", "TestOne");
        testData.MakeTest("TestClass1", "TestTwo", true);
        testData.MakeTest("TestClass1", "TestThree");
        testData.MakeTest("TestClass2", "TestOne", true);
        testData.MakeTest("TestClass2", "TestFailed", createFailed: true);
        testData.MakeTest("TestClass2", "MixedLineEndingsPass", true, mixLineEndings: true);

        //Act
        var result = TestResultScanner.Scan(testData.FolderPath);

        //Assert
        var output = new Output();
        output.WrapLine(@$"""Passed"" indicates the result returned by {nameof(TestResultScanner)}. ""File Exact Match"" indicates whether the received file exactly matches the approved file on disk.");
        output.WriteLine();
        output.WrapLine("The scanner's comparison is line-ending agnostic, so it will report a match even if the line endings are different. One of the test files has deliberately been written with line ending differences to test this, and should show up as a pass but not an exact match.");
        output.WriteLine();
        var formattedResult = result
            .Select(a => new
            {
                a.Test.TestTypeName,
                a.Test.TestName,
                Approved = Path.GetFileName(a.Test.ApprovedFile),
                a.Passed,
                FileExactMatch = File.Exists(a.Test.ReceivedFile) ? (bool?)(File.ReadAllText(a.Test.ApprovedFile) == File.ReadAllText(a.Test.ReceivedFile)) : null 
            })
            .OrderBy(a => a.TestTypeName)
            .ThenBy(a => a.TestName);
        output.FormatTable(formattedResult);
        output.Report.Verify();
    }
}