using System.Diagnostics;
using System.IO;

namespace ApprovalTests
{
    public class CompareUtil
    {
        [DebuggerHidden]
        public static void CompareFiles(string receivedFile, string approvedFile)
        {
            var startInfos = new[]
            {
                new ProcessStartInfo
                {
                    FileName = "winmergeu",
                    Arguments = $"\"{receivedFile}\" \"{approvedFile}\""
                },
                new ProcessStartInfo
                {
                    FileName = "kompare",
                    Arguments = $"\"{receivedFile}\" \"{approvedFile}\""
                },
                new ProcessStartInfo
                {
                    FileName = FindUtil(),
                    Arguments = $"compare \"{receivedFile}\" \"{approvedFile}\"",
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true
                },
            };

            foreach (var startInfo in startInfos)
            {
                try
                {
                    var process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();
                    break;
                }
                catch
                {
                    //this util would not start. Ignore the error, it probably means the system does not have the tool installed
                }
            }
        }

        private static string FindUtil()
        {
            try
            {
                var assemblyLocation = typeof(CompareUtil).Assembly.Location;
                var utilPath = assemblyLocation;
                while (!string.IsNullOrWhiteSpace(utilPath) && Path.GetFileName(Path.GetDirectoryName(utilPath)) != "ApprovalUtilTests")
                    utilPath = Path.GetDirectoryName(utilPath);
                    
                return CheckPath(utilPath);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string CheckPath(string utilPath)
        {
            var exePath = Path.Combine(utilPath, "ApprovalUtil.exe");
            if (File.Exists(exePath))
                return exePath;

            foreach (var directory in Directory.EnumerateDirectories(utilPath))
            {
                var directoryExePath = CheckPath(directory); 
                if (directoryExePath != null) return directoryExePath;
            }

            return null;
        }
    }
}