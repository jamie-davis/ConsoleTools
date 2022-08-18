using System;

namespace VT100.FullScreen
{
    internal class CursorHider : IDisposable
    {
        public CursorHider()
        {
            Console.Write(VirtualTerminalSequences.HideCursor);
        }
        #region IDisposable

        public void Dispose()
        {
            Console.Write(VirtualTerminalSequences.ShowCursor);
        }

        #endregion
    }
}