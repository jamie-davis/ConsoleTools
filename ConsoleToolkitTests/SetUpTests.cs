using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace ConsoleToolkitTests
{
    public class SetUpTests
    {
        public static void OverrideCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB", true);
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        }
    }
}
