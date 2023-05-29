using System.Text;
using ConsoleToolkit.Properties;

namespace ApprovalUtilTests.TestUtilities;

/// <summary>
/// This class will manage a folder of fake tests to use in testing the result and output scanners. It must be
/// constructed with the path to a host folder. Directories to house the test results will be created in the host
/// folder, and as long as the generator is correctly disposed they will also be cleaned up.
/// </summary>
public class TestOutputGenerator : IDisposable
{
    private readonly string _topLevelFolderPath;
    private TestDataTempFolder? _tempFolder;

    public TestOutputGenerator(string? hostFolder = null)
    {
        if (hostFolder == null)
            _tempFolder = new TestDataTempFolder();
        
        _topLevelFolderPath = Path.Combine(hostFolder ?? _tempFolder.TestFolderPath, "Tests");
        Directory.CreateDirectory(_topLevelFolderPath);
    }

    public string FolderPath => _topLevelFolderPath;

    #region Implementation of IDisposable

    /// <summary>
    /// Attempt to remove the test folder. This will fail if the folder is in use, but repeated attempts will be made.
    /// There is a timeout on making the attempts so it is possible that the folder will not be removed. However, failure
    /// to remove the folder will not throw, and there is still a chance that the folder will be cleaned up when the test
    /// run completes.
    /// </summary>
    public void Dispose()
    {
        TestFolderRemover.Remove(_topLevelFolderPath);
        _tempFolder?.Dispose();
    }

    #endregion

    public void MakeTest(string testClassName, string testName,
        bool createReceived = false, bool createFailed = false, bool mixLineEndings = false, bool createNew = false)
    {
        var testClassFile = Path.Combine(_topLevelFolderPath, testClassName + ".cs");
        File.AppendAllLines(testClassFile, new [] {testClassName, testName});
        
        if (!createNew)
        {
            var approvedFileName = $"{testClassName}.{testName}.approved.txt";
            CopyResource("approved.txt", approvedFileName, mixLineEndings ? "\r\n" : null);
        }

        if (createReceived || createFailed || createNew)
        {
            var receivedFileName = $"{testClassName}.{testName}.received.txt";
            CopyResource(createFailed ? "received.txt" : "approved.txt", receivedFileName, mixLineEndings ? "\n" : null);
        }
    }

    private void CopyResource(string approvedTxt, string outputFileName, string? lineEndingOverride)
    {
        var destFilePath = Path.Combine(_topLevelFolderPath, outputFileName);
        using var stream = GetType().Assembly.GetManifestResourceStream($"ApprovalUtilTests.TestUtilities.{approvedTxt}");
        using var reader = new StreamReader(stream!);
        if (lineEndingOverride == null)
        {
            File.WriteAllText(destFilePath, reader.ReadToEnd());
            return;
        }

        var sb = new StringBuilder();
        while (reader.ReadLine() is { } line)
        {
            sb.Append($"{line}{lineEndingOverride}");
        }
        File.WriteAllText(destFilePath, sb.ToString());
    }
}