namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class ConsoleRedirectTester : IConsoleRedirectTester
    {
        private bool _consoleRedirectionTested;
        private bool _outRedirected;
        private bool _errorRedirected;

        /// <summary>
        /// Determine whether the console's output stream is redirected.
        /// </summary>
        public bool IsOutputRedirected()
        {
            PerformTestIfFirstCall();
            return _outRedirected;
        }

        /// <summary>
        /// Determine whether the console's error stream is redirected.
        /// </summary>
        public bool IsErrorRedirected()
        {
            PerformTestIfFirstCall();
            return _errorRedirected;
        }

        private void PerformTestIfFirstCall()
        {
            if (!_consoleRedirectionTested)
            {
                _outRedirected = ConsoleRedirectionStateDetector.IsOutputRedirected;
                _errorRedirected = ConsoleRedirectionStateDetector.IsErrorRedirected;

                _consoleRedirectionTested = true;
            }
        }
    }
}