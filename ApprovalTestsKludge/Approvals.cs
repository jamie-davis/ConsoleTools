using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ApprovalTests.Exceptions;
using ApprovalUtilities.Utilities;

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
                var approved = FixPlatformLineEndings(File.ReadAllText(approvedFile));
                if (approved != FixPlatformLineEndings(text))
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

        private static string FixPlatformLineEndings(string text)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    sb.AppendLine(line);
            }

            return sb.ToString();
        }

        public static void Verify(StringBuilder text)
        {
            Verify(text.ToString());
        }
    }
}