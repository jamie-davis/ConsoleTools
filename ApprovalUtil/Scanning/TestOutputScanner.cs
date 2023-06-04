using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace ApprovalUtil.Scanning;

/// <summary>
/// Scan a folder for test outputs
/// </summary>
public static class TestOutputScanner
{
    /// <summary>
    /// To be matched, the test outputs must be accompanied by a test source file.
    /// </summary>
    /// <param name="codePath">A folder containing test source code and approval outputs</param>
    /// <returns>A list of approval outputs</returns>
    public static List<ApprovalTestOutput> Scan(string codePath)
    {
        return GetOutputsFromFolder(codePath).ToList();
    }

    /// <summary>
    /// Construct a single test output using only the paths for the received and approved files. The name of the test,
    /// the test class and the source file location will be derived. In the event that these elements cannot be derived
    /// then the output instance will be returned with as much useful data as possible. Any elements that cannot be
    /// determined will be set to an empty string.
    /// </summary>
    /// <param name="receivedFilePath">The path to the received text file for the test. This need not exist.</param>
    /// <param name="approvedFilePath">The path to the approved test file for the test. This need not exist.</param>
    /// <returns>An approval test output instance. This will be returned regardless of the validity of the text files
    /// but its content will be as useful as it can be made to be.</returns>
    public static ApprovalTestOutput OutputFromResultFiles(string? receivedFilePath, string? approvedFilePath)
    {
        var (testClass, testMethod) = ApprovalFileNameDeconstructor.Deconstruct(receivedFilePath);
        
        return new ApprovalTestOutput(string.Empty, approvedFilePath, receivedFilePath, testClass ?? string.Empty, testMethod ?? string.Empty);
    }

    private static IEnumerable<ApprovalTestOutput> GetOutputsFromFolder(string codePath)
    {
        foreach (var output in GetOutputs(codePath))
            yield return output;

        foreach (var directory in Directory.EnumerateDirectories(codePath))
        {
            foreach (var subDirectoryOutput in GetOutputsFromFolder(directory))
                yield return subDirectoryOutput;
        }
    }

    private static IEnumerable<ApprovalTestOutput> GetOutputs(string codePath)
    {
        var approvalFiles = Directory.EnumerateFiles(codePath, "*.approved.txt").Concat(Directory.EnumerateFiles(codePath, "*.received.txt"));
        var testTypesAndNames = approvalFiles.Select(f => new {Path = Path.GetDirectoryName(f), TestTypeAndName = ApprovalFileNameDeconstructor.Deconstruct(Path.GetFileName(f))})
            .Where(t => t.TestTypeAndName is { TestTypeName: not null, TestName: not null })
            .Distinct()
            .ToList();
        foreach (var testDetails in testTypesAndNames)
        {
            var testTypeName = testDetails.TestTypeAndName.TestTypeName!;
            var testName = testDetails.TestTypeAndName.TestName!;
            var sourceFile = FindSourceForTest(testDetails.Path!, testTypeName, testName);
            if (sourceFile == null) continue;
            
            var receivedFileName = Path.Combine(testDetails.Path!, $"{testTypeName}.{testName}.received.txt");
            var receivedFile = File.Exists(receivedFileName) ? receivedFileName : null;

            var approvedFileName = Path.Combine(testDetails.Path!, $"{testTypeName}.{testName}.approved.txt");
            var approvedFile = File.Exists(approvedFileName) ? approvedFileName : null;
            yield return new ApprovalTestOutput(sourceFile, approvedFile, receivedFile, testTypeName, testName);
        }
    }

    private static string? FindSourceForTest(string codePath, string testTypeName, string testName)
    {
        foreach (var codeFile in Directory.EnumerateFiles(codePath))
        {
            if (ApprovalFileNameDeconstructor.Deconstruct(codeFile).TestTypeName != null) continue; //exclude approval outputs

            var code = File.ReadAllText(codeFile);
            if (code.Contains(testTypeName) && code.Contains(testName))
                return codeFile;
        }

        return null;
    }
}