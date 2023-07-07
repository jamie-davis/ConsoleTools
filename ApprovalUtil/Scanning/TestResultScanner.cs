using ApprovalTests.Tools;

namespace ApprovalUtil.Scanning;

/// <summary>
/// Extract approval test results from source code folders
/// </summary>
public static class TestResultScanner
{
    /// <summary>
    /// This returns a list of all of the approval tests located and their results.
    /// </summary>
    /// <param name="pathToCode"></param>
    /// <returns></returns>
    public static List<ApprovalTestResult> Scan(string pathToCode)
    {
        var tests = TestOutputScanner.Scan(pathToCode);
        return tests.Select(t => new ApprovalTestResult(t, IsMatch(t))).ToList();
    }

    public static bool IsMatch(ApprovalTestOutput test)
    {
        var receivedMissing = string.IsNullOrEmpty(test.ReceivedFile) || !File.Exists(test.ReceivedFile);
        var approvedMissing = string.IsNullOrEmpty(test.ApprovedFile) || !File.Exists(test.ApprovedFile);
        if (receivedMissing)
            return !approvedMissing; //received is missing but if approved is present, it's a match

        if (approvedMissing)
            return false;
        
        var approved = PlatformLineEndingFixer.Fix(File.ReadAllText(test.ApprovedFile));
        var received = PlatformLineEndingFixer.Fix(File.ReadAllText(test.ReceivedFile));
        return approved == received;
    }
}