using ApprovalUtil.Scanning;
using ConsoleToolkit.ConsoleIO;

namespace ApprovalUtil.Approving;

internal static class TestOutputLoader
{
    internal static (string? Received, string? Approved) LoadText(ApprovalTestOutput test, IErrorAdapter error)
    {
        string? ReadFileText(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return string.Empty;

            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                error.WrapLine($"Error reading {filePath}".Red());
                error.WrapLine(e.Message.Red());
                return null;
            }
        }

        var receivedFile = test.ReceivedFile;
        var approvedFile = test.ApprovedFile;

        return (ReadFileText(receivedFile), ReadFileText(approvedFile));
    }

}