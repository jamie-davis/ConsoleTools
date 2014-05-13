namespace ConsoleToolkit.ConsoleIO.Internal.WidthCalculators
{
    /// <summary>
    /// Calculate the defaults for String column widths.
    /// </summary>
    public static class StringWidthCalculator
    {
        public static int Min(ColumnFormat format)
        {
            return ValueFormatter.Format(format, string.Empty).Length;
        }
    }
}