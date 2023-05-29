using ApprovalUtil.Approving;
using ApprovalUtil.Scanning;
using ApprovalUtilTests.TestDoubles;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using TestConsoleLib.Testing;

namespace ApprovalUtilTests.Approving;

public class InteractiveApproverTests
{
    [Fact]
    public void InteractiveApproverDescribesResultsAndShowsMenu()
    {
        //Arrange
        var testConsole = new UnitTestConsole(typeof(Program).Namespace);
        var console = testConsole.Console;
        var error = testConsole.Error;

        ApprovalTestResult MakeTestResult(string testTypeName, string testName, bool passed)
        {
            var approvedFile = $"{testTypeName}.{testName}.approved.txt";
            var receivedFile = $"{testTypeName}.{testName}.received.txt";
            var sourceFile = $"{testTypeName}.cs";
            var approvalTestOutput = new ApprovalTestOutput(sourceFile, approvedFile, receivedFile, testTypeName, testName);
            return new ApprovalTestResult(approvalTestOutput, passed);
        }

        var tests = new List<ApprovalTestResult>()
        {
            MakeTestResult("TestClass", "TestOne", true),
            MakeTestResult("TestClass", "TestTwo", true),
            MakeTestResult("TestClass", "TestThree", false),
            MakeTestResult("TestClass", "TestFour", true),
        };
        
        var approverParams = new FakeApproverParams { Interactive = true };
        var data = @"X
";
        using var stream = new StringReader(data);
        testConsole.Interface.SetInputStream(stream);

        //Act
        var _ = InteractiveApprover.Execute(console, error, tests, approverParams);

        //Assert
        testConsole.Interface.GetBuffer(ConsoleBufferFormat.TextOnly).Verify();
    }
}