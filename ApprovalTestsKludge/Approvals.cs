using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ApprovalTests.Exceptions;
using ApprovalTests.Tools;

namespace ApprovalTests
{
    public static class Approvals
    {
        public static void Verify(string text)
        {
            var frame = FindStackFrame();
            Debug.WriteLine(frame.GetFileName());
            var path = Path.GetDirectoryName(frame.GetFileName());
            var method = frame.GetMethod();

            var baseFileName = $"{method.DeclaringType.Name}.{method.Name}";
            var receivedOutput = $"{baseFileName}.received.txt";
            var approvedOutput = $"{baseFileName}.approved.txt";
            var approvedFile = Path.Combine(path, approvedOutput);
            var receivedFile = Path.Combine(path, receivedOutput);
            if (File.Exists(approvedFile))
            {
                var approved = PlatformLineEndingFixer.Fix(File.ReadAllText(approvedFile));
                if (approved != PlatformLineEndingFixer.Fix(text))
                {
                    File.WriteAllText(receivedFile, text);
                    CompareUtil.CompareFiles(receivedFile, approvedFile);
                    throw new ApprovedFileMismatchException();
                }
                
                if (File.Exists(receivedFile))
                    File.Delete(receivedFile);
                return;
            } 
            
            File.WriteAllText(receivedFile, text);
            File.WriteAllText(approvedFile, string.Empty);
            CompareUtil.CompareFiles(receivedFile, approvedFile);
            throw new NoApprovedFileException();
        }

        private static StackFrame FindStackFrame()
        {
            var stackTrace = new StackTrace(true);
            Debug.Assert(stackTrace != null);
            var frames = stackTrace.GetFrames();
            Debug.Assert(frames != null);
            return frames.Select(f => new { Frame = f, Method = f.GetMethod()})
                .First(m => m.Method.DeclaringType != typeof(Approvals))
                .Frame;
        }

        public static void Verify(StringBuilder text)
        {
            Verify(text.ToString());
        }
    }
}