namespace ApprovalUtil.Scanning;

/// <summary>
/// The located output from an approval test
/// </summary>
public class ApprovalTestOutput
{
    public ApprovalTestOutput(string sourceFile, string? approvedFile, string? receivedFile, string testTypeName, string testName)
    {
        SourceFile = sourceFile;
        ApprovedFile = approvedFile;
        ReceivedFile = receivedFile;
        TestTypeName = testTypeName;
        TestName = testName;
    }

    public string SourceFile { get; }
    public string? ApprovedFile { get; }
    public string? ReceivedFile { get; }
    public string TestTypeName { get; }
    public string TestName { get; }
}