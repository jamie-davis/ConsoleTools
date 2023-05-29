namespace ApprovalUtil.Scanning;

/// <summary>
/// An approval test result located in the source code.
/// </summary>
public class ApprovalTestResult
{
    public ApprovalTestResult(ApprovalTestOutput test, bool passed)
    {
        Test = test;
        Passed = passed;
    }

    public ApprovalTestOutput Test { get; }
    public bool Passed { get; }
}