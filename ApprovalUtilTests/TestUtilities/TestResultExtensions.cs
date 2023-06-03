using ApprovalUtil.Scanning;

namespace ApprovalUtilTests.TestUtilities;

internal static class TestResultExtensions
{
    public static ApprovalTestResult FirstNew(this List<ApprovalTestResult> results)
    {
        return results.First(t => File.Exists(t.Test.ReceivedFile)
                                  && !File.Exists(t.Test.ApprovedFile));
    }

    public static ApprovalTestResult FirstFailure(this List<ApprovalTestResult> results)
    {
        return results.First(t => File.Exists(t.Test.ReceivedFile)
                                  && File.Exists(t.Test.ApprovedFile) 
                                  && !TestResultScanner.IsMatch(t.Test));
    }
    
    public static ApprovalTestResult FirstApproved(this List<ApprovalTestResult> results)
    {
        return results.First(t => !File.Exists(t.Test.ReceivedFile)
                                  && File.Exists(t.Test.ApprovedFile));
    }
}