namespace ConsoleToolkit.InteractiveSession
{
    /// <summary>
    /// This interface defines a service that commands can use to switch into an interactive session. This is a session
    /// in which commands are taken from the input stream, parsed and executed, one after another.
    /// </summary>
    public interface IInteractiveSessionService
    {
        void BeginSession();
        void EndSession();
    }
}