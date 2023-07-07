using ApprovalUtil.Scanning;

namespace ApprovalUtilTests.TestUtilities;

internal static class TestOutputExtensions
{
    public static ApprovalTestOutput FirstNew(this List<ApprovalTestOutput> results)
    {
        return results.First(t => File.Exists(t.ReceivedFile)
                                  && !File.Exists(t.ApprovedFile));
    }

    public static ApprovalTestOutput FirstFailure(this List<ApprovalTestOutput> results)
    {
        return results.First(t => File.Exists(t.ReceivedFile)
                                  && File.Exists(t.ApprovedFile)
                                  && !TestResultScanner.IsMatch(t));
    }

    public static ApprovalTestOutput FirstApproved(this List<ApprovalTestOutput> results)
    {
        return results.First(t => !File.Exists(t.ReceivedFile)
                                  && File.Exists(t.ApprovedFile));
    }
}