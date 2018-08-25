using System;
#if NET40
using System.Runtime.InteropServices;
#endif

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class ConsoleRedirectionStateDetector
    {
        public static bool IsInputRedirected
        {
            get 
            {
#if NET40
                 return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdin));
#else
                return Console.IsInputRedirected;
#endif
            }
        }

        public static bool IsErrorRedirected
        {
            get
            {
#if NET40
                return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stderr));
#else
                return Console.IsErrorRedirected;
#endif
            }
        }

        public static bool IsOutputRedirected
        {
            get
            {
#if NET40
                return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdout));
#else
                return Console.IsOutputRedirected;
#endif
            }
        }

#if NET40
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
#endif
    }
}
