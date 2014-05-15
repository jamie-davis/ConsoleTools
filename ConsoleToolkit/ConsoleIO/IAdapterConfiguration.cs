namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This interface defines the parameters that can be configured for a <see cref="IConsoleAdapter"/>.
    /// </summary>
    public interface IAdapterConfiguration
    {
        char ControlSequenceIntroducer { get; set; }
        char ControlSequenceTerminator { get; set; }
    }
}