using System;
using System.Runtime.InteropServices;
using VT100.Exceptions;

namespace VT100.Utilities
{
    internal class RequireVTMode : IDisposable, IVTModeControl
    {
        private IntPtr _stdInHandle;
        private IntPtr _stdOutHandle;

        #region Win32 definitions

        // ReSharper disable InconsistentNaming

        private const int STD_INPUT_HANDLE = -10;

        private const int STD_OUTPUT_HANDLE = -11;

        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;
        
        private const uint ENABLE_INSERT_MODE = 0x0020;

        private static readonly uint UNICODE_CP = (uint)System.Text.Encoding.UTF8.CodePage;

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
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetConsoleOutputCP();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetConsoleCP();

        enum CtrlType {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private delegate bool EventHandler(CtrlType sig);

        private uint _inConsoleMode;
        private uint _outConsoleMode;
        private IntPtr _iStdInHandle;
        private IntPtr _iStdOutHandle;
        private bool _insertModeOn;
        private uint _consoleCP; 
        private uint _consoleOutputCP; 

        #endregion

        public RequireVTMode(bool setAutomatically = true)
        {
            if (setAutomatically)
            {
                if (!TryApply())
                {
                    throw new VTModeUnavailableException(LastError, LastErrorCode);
                }
            }
        }

        public bool TryApply()
        {
            SetConsoleCtrlHandler(HandleConsoleCtrl, true);

            _stdInHandle = GetStdHandle(STD_INPUT_HANDLE);
            _stdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            _consoleCP = GetConsoleCP();
            _consoleOutputCP = GetConsoleOutputCP();

            if (!GetConsoleMode(_stdInHandle, out uint inConsoleMode))
            {
                LastError = "failed to get input console mode";
                LastErrorCode = 0;
                return false;
            }

            if (!GetConsoleMode(_stdOutHandle, out uint outConsoleMode))
            {
                LastError = "failed to get output console mode";
                LastErrorCode = 0;
                return false;
            }

            BaseInputConsoleMode = inConsoleMode;
            BaseOutputConsoleMode = outConsoleMode;

            inConsoleMode |= ENABLE_VIRTUAL_TERMINAL_INPUT;
            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;

            RequiredInputConsoleMode = inConsoleMode;
            RequiredOutputConsoleMode = outConsoleMode;

            if (inConsoleMode == BaseInputConsoleMode && outConsoleMode == BaseOutputConsoleMode)
                return true;

            if (!SetConsoleMode(_stdInHandle, inConsoleMode))
            {
                var lastError = GetLastError();
                LastError = $"failed to set input console mode, error code: {lastError}";
                LastErrorCode = lastError;
                return false;
            }

            if (!SetConsoleMode(_stdOutHandle, outConsoleMode))
            {
                var lastError = GetLastError();
                ResetInConsoleMode();
                LastError = $"failed to set output console mode, error code: {lastError}";
                LastErrorCode = lastError;
                return false;
            }

            if (!SetConsoleOutputCP(UNICODE_CP))
            {
                var lastError = GetLastError();
                ResetInConsoleMode();
                ResetOutConsoleMode();
                LastError = $"failed to set console output code page, error code: {lastError}";
                return false;
            }

            if (!SetConsoleCP(UNICODE_CP))
            {
                var lastError = GetLastError();
                ResetInConsoleMode();
                ResetOutConsoleMode();
                ResetOutputCP();
                LastError = $"failed to set console code page, error code: {lastError}";
                Console.ReadKey();
                return false;
            }

            
            ModeWasSet = true;
            return true;
        }

        /// <summary>
        /// Fetch the current console insert/overwrite mode.
        /// </summary>
        public bool InsertModeOn
        {
            get
            {
                if (GetConsoleMode(_stdInHandle, out var mode))
                    return (mode & ENABLE_INSERT_MODE) > 0;
                return true;
            }
        }

        /// <summary>
        /// The console application is being terminated, so we need to reset the console mode.
        /// </summary>
        private bool HandleConsoleCtrl(CtrlType sig)
        {
            ResetInConsoleMode();
            ResetOutConsoleMode();
            return false; //allow the termination to happen. returning true would block it.
        }

        private void ResetInConsoleMode()
        {
            SetConsoleMode(_stdInHandle, BaseInputConsoleMode);
        }

        private void ResetOutConsoleMode()
        {
            SetConsoleMode(_stdOutHandle, BaseOutputConsoleMode);
        }

        private void ResetOutputCP()
        {
            SetConsoleOutputCP(_consoleOutputCP);
        }

        private void ResetCP()
        {
            SetConsoleCP(_consoleCP);
        }


        public uint BaseInputConsoleMode { get; private set; }
        public uint BaseOutputConsoleMode { get; private set; }
        public uint RequiredInputConsoleMode { get; set; }
        public uint RequiredOutputConsoleMode { get; set; }

        public string LastError { get; private set; }
        public uint LastErrorCode { get; private set; }

        public bool ModeWasSet { get; private set; }

        public void Dispose()
        {
            if (ModeWasSet)
            {
                ResetInConsoleMode();
                ResetOutConsoleMode();
                ResetOutputCP();
                ResetCP();
            }
        }

        #region Implementation of IVTModeControl

        public void ToggleInsertMode()
        {
            if (GetConsoleMode(_stdInHandle, out var mode))
                SetConsoleMode(_stdInHandle, mode ^ ENABLE_INSERT_MODE);
        }

        #endregion
    }
}
