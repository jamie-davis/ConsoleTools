using System;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// This class accepts an exception from a background thread, and can be checked from the foreground thread, where
    /// the exception can be re-thrown.<para/>
    /// 
    /// The reason for this class to exist is to allow exceptions to be captured when a process feeds data
    /// into a consuming enumerable in a background thread, which can be processed as the collection is populated
    /// without the processing application having to be aware that there is a multi-threaded operation in progress.
    /// <remarks>As an example, this technique is used to implement the <see cref="ConsoleAdapter.FormatTable{T}(System.Collections.Generic.IEnumerable{T},ConsoleToolkit.ConsoleIO.ReportFormattingOptions,string)"/>
    /// method of the <see cref="ConsoleAdapter"/>. When a table is formatted, the output appears as the source
    /// is processed, rather than waiting until all of the input rows are available.</remarks>
    /// </summary>
    public class ReportExceptionFilter
    {
        private object _lock = new object();
        private bool _exceptionCaptured;
        private Exception _exception;

        public void AddException(Exception exception)
        {
            lock(_lock)
            {
                _exceptionCaptured = true;
                _exception = exception;
            }
        }

        public bool ExceptionCaptured
        {
            get { lock (_lock) return _exceptionCaptured; }
        }

        public Exception Exception
        {
            get { lock (_lock) return _exception; }
        }
    }
}
