using System.IO;
using System.Text;

namespace ApprovalTests.Tools
{
    public static class PlatformLineEndingFixer
    {
        public static string Fix(string text)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}