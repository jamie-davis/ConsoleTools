using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
    }
}