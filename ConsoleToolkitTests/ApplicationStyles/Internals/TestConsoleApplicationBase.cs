using System;
using System.Linq;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using CommandLoadTestAssembly;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ApplicationStyles.Internals
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestConsoleApplicationBase
    {
        #region Types for test

        public class TestConsoleApp : ConsoleApplicationBase
        {
            [Command]
            public class Command
            {
                [CommandHandler]
                public void Handle()
                {}
            }

            public TestConsoleApp()
            {
                SetConfigTypeFilter(t => t.DeclaringType == typeof(TestConsoleApp));
            }

            public void CallPostInit()
            {
                PostInitialise();
            }

            internal override void LoadConfigFromAssembly()
            {
                Config = new CommandLineInterpreterConfiguration();
            }
        }

        #endregion

        [Test]
        public void MultiplePostInitsThrow()
        {
            Assert.Throws<CallOrderViolationException>(() =>
            {
                var app = new TestConsoleApp();
                app.CallPostInit();
                app.CallPostInit(); //second call is invalid.
            });
        }

        [Test]
        public void CommandsAreLocatedPostInit()
        {
            Program.XMain(new []{"one"});
            var commands = Program.LastProgram.GetConfig().Commands.Select(c => c.Name).JoinWith(",");
            Assert.That(commands, Is.EqualTo("one,two,three,four,ione,itwo,none,ntwo"));
        }

        [Test]
        public void CommandsHandlersAreLocatedPostInit()
        {
            Program.XMain(new []{"one"});
            var commands = Program.LastProgram.Handlers.Select(c => c.Value.CommandType.Name).JoinWith(",");
            Assert.That(commands, Is.EqualTo("Command4,Command1,Command2,Command3"));
        }

        [Test]
        public void HandlerShouldBeInvoked()
        {
            Program.XMain(new []{"three"});
            var result = Program.Executed3;
            Assert.That(result, Is.True);
        }
    }
}