using System.Diagnostics;

namespace ApprovalTests
{
    public class CompareUtil
    {
        public static void CompareFiles(string receivedFile, string approvedFile)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "kompare",
                Arguments = $"\"{receivedFile}\" \"{approvedFile}\""
            };
            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}