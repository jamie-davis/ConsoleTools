using System;

namespace VT100.Exceptions
{
    internal class VTModeUnavailableException : Exception
    {
        public string Error { get; }
        public uint ErrorCode { get; }

        public VTModeUnavailableException(string error, uint errorCode) : base("VT mode could not be set")
        {
            Error = error;
            ErrorCode = errorCode;
        }
    }
}