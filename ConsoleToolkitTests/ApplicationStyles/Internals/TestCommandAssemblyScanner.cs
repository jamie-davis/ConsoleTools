using System.Linq;
using System.Reflection;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using CommandLoadTestAssembly;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ApplicationStyles.Internals
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandAssemblyScanner
    {
        private Assembly _assembly;

        [SetUp]
        public void SetUp()
        {
            _assembly = typeof (Program).Assembly;
        }

        [Test]
        public void AllOfTheCommandTypesAreExtracted()
        {
            var commandTypes = CommandAssemblyScanner.FindCommands(_assembly);
            var result = commandTypes.Select(t => t.Name).JoinWith(",");
            Assert.That(result, Is.EqualTo("Command1,Command2,Command3,Command4"));
        }

        [Test]
        public void AllOfTheCommandHandlerTypesAreExtracted()
        {
            var types = CommandAssemblyScanner.FindCommandHandlers(_assembly);
            var result = types.Select(t => t.Name).JoinWith(",");
            Assert.That(result, Is.EqualTo("Command1Handler,Command2Handler,Command3Handler"));
        }
    }
}
