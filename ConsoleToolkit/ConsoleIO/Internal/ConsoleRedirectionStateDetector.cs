using System;
using System.Runtime.InteropServices;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    #if TARGET_FRAMEWORK_4

    internal static class ConsoleRedirectionStateDetector
    {
        public static bool IsInputRedirected
        {
            get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdin)); }
        }

        public static bool IsErrorRedirected
        {
            get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stderr)); }
        }

        public static bool IsOutputRedirected
        {
            get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdout)); }
        }

        // P/Invoke:
        private enum FileType
        {
            Unknown,
            Disk,
            Char,
            Pipe
        };

        private enum StdHandle
        {
            Stdin = -10,
            Stdout = -11,
            Stderr = -12
        };

        [DllImport("kernel32.dll")]
        private static extern FileType GetFileType(IntPtr hdl);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);
    }

    #endif
}
