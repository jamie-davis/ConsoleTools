using ApprovalTests.Reporters;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (DiffReporter))]
    public class TestConsoleAdapter
    {
        
    }
}