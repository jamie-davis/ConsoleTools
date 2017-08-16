using System.Collections.Generic;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal class KeywordsDesc
    {
        public string Description { get; private set; }
        public List<string> Keywords { get; private set; }

        public KeywordsDesc(string description, List<string> keywords)
        {
            Description = description;
            Keywords = keywords;
        }
    }
}