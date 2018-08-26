using System.Linq;
using System.Reflection;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
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
            _assembly = typeof (CommandLoadTestAssembly.Program).Assembly;
        }

        [Test]
        public void AllOfTheCommandTypesAreExtracted()
        {
            var commandTypes = CommandAssemblyScanner.FindCommands(_assembly, CommandScanType.AllCommands)
                .Where(t => t.Namespace == typeof(CommandLoadTestAssembly.Program).Namespace);
            var result = commandTypes.Select(t => t.Name).JoinWith(",");
            Assert.That(result, Is.EqualTo("Command1,Command2,Command3,Command4,InteractiveCommand1,InteractiveCommand2,NonInteractiveCommand1,NonInteractiveCommand2"));
        }

        [Test]
        public void TheInteractiveCommandTypesAreExtracted()
        {
            var commandTypes = CommandAssemblyScanner.FindCommands(_assembly, CommandScanType.InteractiveCommands)
                .Where(t => t.Namespace == typeof(CommandLoadTestAssembly.Program).Namespace);
            var result = commandTypes.Select(t => t.Name).JoinWith(",");
            Assert.That(result, Is.EqualTo("Command1,Command2,Command3,Command4,InteractiveCommand1,InteractiveCommand2"));
        }

        [Test]
        public void TheNonInteractiveCommandTypesAreExtracted()
        {
            var commandTypes = CommandAssemblyScanner.FindCommands(_assembly, CommandScanType.NonInteractiveCommands)
                .Where(t => t.Namespace == typeof(CommandLoadTestAssembly.Program).Namespace);
            var result = commandTypes.Select(t => t.Name).JoinWith(",");
            Assert.That(result, Is.EqualTo("Command1,Command2,Command3,Command4,NonInteractiveCommand1,NonInteractiveCommand2"));
        }

        [Test]
        public void AllOfTheCommandHandlerTypesAreExtracted()
        {
            var types = CommandAssemblyScanner.FindCommandHandlers(_assembly)
                .Where(t => t.Namespace == typeof(CommandLoadTestAssembly.Program).Namespace);
            var result = types.Select(t => t.Name).JoinWith(",");
            Assert.That(result, Is.EqualTo("Command1Handler,Command2Handler,Command3Handler"));
        }
    }
}
