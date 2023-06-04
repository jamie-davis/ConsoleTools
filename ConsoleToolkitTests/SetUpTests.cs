using System.Globalization;
using System.Threading;
using Xunit;
using System.Runtime.CompilerServices;

[assembly: CollectionBehavior (CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace ConsoleToolkitTests
{
    public class SetUpTests
    {
        [ModuleInitializer]    
        public static void OverrideCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB", true);
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        }
    }
}
