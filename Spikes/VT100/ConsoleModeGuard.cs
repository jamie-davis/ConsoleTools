using System;
using System.Runtime.InteropServices;

namespace Vt100
{
    class ConsoleModeGuard : IDisposable
    {
        private const int STD_INPUT_HANDLE = -10;

        private const int STD_OUTPUT_HANDLE = -11;

        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;

        private const uint ENABLE_LINE_INPUT = 0x0002;
        
        enum CtrlType {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        // ReSharper restore InconsistentNaming

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private uint _inConsoleMode;
        private uint _outConsoleMode;
        private IntPtr _iStdInHandle;
        private IntPtr _iStdOutHandle;

        public ConsoleModeGuard()
        {
            SetConsoleCtrlHandler(HandleConsoleCtrl, true);

            _iStdInHandle = GetStdHandle(STD_INPUT_HANDLE);
            _iStdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);

            if (!GetConsoleMode(_iStdInHandle, out _inConsoleMode))
            {
                Console.WriteLine("failed to get input console mode");
                Console.ReadKey();
                return;
            }
            if (!GetConsoleMode(_iStdOutHandle, out _outConsoleMode))
            {
                Console.WriteLine("failed to get output console mode");
                Console.ReadKey();
                return;
            }

            var enabledInConsoleMode = _inConsoleMode | ENABLE_VIRTUAL_TERMINAL_INPUT;
            var enabledOutConsoleMode = _outConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;

            if (!SetConsoleMode(_iStdInHandle, enabledInConsoleMode))
            {
                Console.WriteLine($"failed to set input console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }
            if (!SetConsoleMode(_iStdOutHandle, enabledOutConsoleMode))
            {
                Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }

        }

        /// <summary>
        /// The console application is being terminated, so we need to reset the console mode.
        /// </summary>
        private bool HandleConsoleCtrl(CtrlType sig)
        {
            ResetConsoleMode();
            return false; //allow the termination to happen. returning true would block it.
        }

        #region IDisposable

        public void Dispose()
        {
            ResetConsoleMode();
        }

        private void ResetConsoleMode()
        {
            if (!SetConsoleMode(_iStdInHandle, _inConsoleMode))
            {
                Console.WriteLine($"failed to set input console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }

            if (!SetConsoleMode(_iStdOutHandle, _outConsoleMode))
            {
                Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }
        }

        #endregion
    }
}