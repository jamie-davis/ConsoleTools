namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// The global adapter configuration.
    /// </summary>
    public static class AdapterConfiguration
    {
        public static char ControlSequenceIntroducer { get; set; }
        public static char ControlSequenceTerminator { get; set; }

        static AdapterConfiguration()
        {
            ControlSequenceIntroducer = '\uE000';
            ControlSequenceTerminator = '\uE001';
        }
    }
}