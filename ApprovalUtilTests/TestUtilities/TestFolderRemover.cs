using System.Diagnostics;

namespace ApprovalUtilTests.TestUtilities;

public static class TestFolderRemover
{
    private const int MaxDurationDeleteAttemptsMs = 1000;

    /// <summary>
    /// Remove a test output folder, but we only try for a limited amount of time defined by the constant
    /// <see cref="MaxDurationDeleteAttemptsMs"/>. No exceptions are thrown if the folder cannot be removed, but messages
    /// are written to debug output.
    /// </summary>
    internal static void Remove(string path)
    {
        var sw = new Stopwatch();
        sw.Start();
        do
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
                return;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to remove test output folder {path} due to error: {e.Message}");
                Thread.Sleep(250);
            }
        } while (sw.ElapsedMilliseconds < MaxDurationDeleteAttemptsMs);
        
        Debug.WriteLine($"Unable to remove {path}");

    }
}