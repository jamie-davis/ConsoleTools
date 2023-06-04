using System.Linq;
using System.Reflection;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ApplicationStyles.Internals
{
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandAssemblyScanner
    {
        private Assembly _assembly;
        public TestCommandAssemblyScanner()
        {
            _assembly = typeof (CommandLoadTestAssembly.Program).Assembly;
        }

        [Fact]
        public void AllOfTheCommandTypesAreExtracted()
        {
            var commandTypes = CommandAssemblyScanner.FindCommands(_assembly, CommandScanType.AllCommands)
                .Where(t => t.Namespace == typeof(CommandLoadTestAssembly.Program).Namespace);
            var result = commandTypes.Select(t => t.Name).JoinWith(",");
            Assert.Equal("Command1,Command2,Command3,Command4,InteractiveCommand1,InteractiveCommand2,NonInteractiveCommand1,NonInteractiveCommand2", result);
        }

        [Fact]
        public void TheInteractiveCommandTypesAreExtracted()
        {
            var commandTypes = CommandAssemblyScanner.FindCommands(_assembly, CommandScanType.InteractiveCommands)
                .Where(t => t.Namespace == typeof(CommandLoadTestAssembly.Program).Namespace);
            var result = commandTypes.Select(t => t.Name).JoinWith(",");
            Assert.Equal("Command1,Command2,Command3,Command4,InteractiveCommand1,InteractiveCommand2", result);
        }

        [Fact]
        public void TheNonInteractiveCommandTypesAreExtracted()
        {
            var commandTypes = CommandAssemblyScanner.FindCommands(_assembly, CommandScanType.NonInteractiveCommands)
                .Where(t => t.Namespace == typeof(CommandLoadTestAssembly.Program).Namespace);
            var result = commandTypes.Select(t => t.Name).JoinWith(",");
            Assert.Equal("Command1,Command2,Command3,Command4,NonInteractiveCommand1,NonInteractiveCommand2", result);
        }

        [Fact]
        public void AllOfTheCommandHandlerTypesAreExtracted()
        {
            var types = CommandAssemblyScanner.FindCommandHandlers(_assembly)
                .Where(t => t.Namespace == typeof(CommandLoadTestAssembly.Program).Namespace);
            var result = types.Select(t => t.Name).JoinWith(",");
            Assert.Equal("Command1Handler,Command2Handler,Command3Handler", result);
        }
    }
}
