using ConsoleToolkit.ConsoleIO.Testing;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This enumeration is used when retrieving the buffer from the <see cref="ConsoleInterfaceForTesting"/>
    /// to specify what information should be retrieved.
    /// </summary>
    public enum ConsoleBufferFormat
    {
        TextOnly,
        Interleaved,
        ColourOnly
    }
}