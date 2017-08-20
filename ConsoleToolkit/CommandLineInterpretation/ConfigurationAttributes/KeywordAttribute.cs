using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a command class (see <see cref="CommandAttribute"/>) in order to specify a keyword that should prefix the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KeywordAttribute : Attribute
    {
        private string _description;

        /// <summary>
        /// The keyword(s).
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// The help description to attach to the keyword or keywords.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Constructor specifying keyword(s) and help text for the command.
        /// </summary>
        /// <param name="keyword"></param>
        public KeywordAttribute(string keyword, string description = null)
        {
            Keyword = keyword;
            Description = description;
        }
    }
}