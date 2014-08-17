namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// The definitaion of an option for input purposes.
    /// </summary>
    public class OptionDefinition
    {
        public string Prompt { get; set; }
        public string RequiredValue { get; set; }
        public object SelectedValue { get; set; }
    }
}