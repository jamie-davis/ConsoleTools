namespace ApprovalUtilTests.TestUtilities;

internal class TestDataTempFolder : IDisposable
{
    private readonly DirectoryInfo _rootFolder;

    public TestDataTempFolder()
    {
        Console.WriteLine("Starting up");
        var tempFile = Path.GetTempFileName();
        File.Delete(tempFile);
        _rootFolder = Directory.CreateDirectory(tempFile);
    }

    public string TestFolderPath => _rootFolder.FullName;

    #region Implementation of IDisposable

    public void Dispose()
    {
        Console.WriteLine("Cleaning up");
        if (Directory.Exists(_rootFolder.FullName))
            TestFolderRemover.Remove(_rootFolder.FullName);            
    }

    #endregion
}