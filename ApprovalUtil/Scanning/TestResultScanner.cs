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
        if (string.IsNullOrEmpty(test.ReceivedFile) || !File.Exists(test.ReceivedFile))
            return true;

        if (string.IsNullOrEmpty(test.ApprovedFile) || !File.Exists(test.ApprovedFile))
            return false;
        
        var approved = PlatformLineEndingFixer.Fix(File.ReadAllText(test.ApprovedFile));
        var received = PlatformLineEndingFixer.Fix(File.ReadAllText(test.ReceivedFile));
        return approved == received;
    }
}