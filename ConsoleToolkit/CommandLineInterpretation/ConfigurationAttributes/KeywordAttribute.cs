using System;

namespace ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes
{
    /// <summary>
    /// This attribute decorates a command class (see <see cref="CommandAttribute"/>) in order to specify a keyword that should prefix the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KeywordAttribute : Attribute
    {
        /// <summary>
        /// The keyword.
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// Constructor specifying the keyword for the command.
        /// </summary>
        /// <param name="keyword"></param>
        public KeywordAttribute(string keyword)
        {
            Keyword = keyword;
        }
    }
}