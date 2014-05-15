using System.Linq;

namespace ConsoleToolkitTests.ConsoleIO.UnitTestUtilities
{
    internal static class RulerFormatter
    {
        public static string MakeRuler(int width)
        {
            return string.Join(string.Empty, Enumerable.Range(0, (width/10) + 1).Select(i => "----+----|"))
                .Substring(0, width);
        }
    }
}
