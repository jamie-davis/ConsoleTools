using System;

namespace VT100.FullScreen
{
    internal class ScreenCapture : IDisposable
    {
        private bool _disposed;

        public ScreenCapture()
        {
            Console.Write(VirtualTerminalSequences.UseAlternateScreenBuffer);
        }


        #region IDisposable

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Console.Write(VirtualTerminalSequences.UseMainScreenBuffer);
        }

        #endregion
    }
}