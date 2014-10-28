using System;
using System.Globalization;
using System.Threading;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Temporarily impose a change of culture within a using block. On disposal, the original culture is restored.
    /// Note that behaviour is undefined if the culture is manually changed within the using block.
    /// </summary>
    internal class TempCulture : IDisposable
    {
        private CultureInfo _restoreCulture;
        private Thread _thread;

        public TempCulture(CultureInfo info)
        {
            _thread = Thread.CurrentThread;
            _restoreCulture = _thread.CurrentCulture;
            _thread.CurrentCulture = info;
        }

        public void Dispose()
        {
            _thread.CurrentCulture = _restoreCulture;
        }
    }
}