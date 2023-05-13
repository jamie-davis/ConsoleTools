using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Exceptions;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;
using DescriptionAttribute = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace ConsoleToolkitTests.ApplicationStyles
{
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandDrivenApplication : IDisposable
    {
        private ConsoleInterfaceForTesting _console;

        #region Types for test
        #pragma warning disable 649

        public class TestApp : CommandDrivenApplication
        {
            private static bool _setExitCodeInCommandLineValid;
            public static TestApp LastTestApp { get; set; }
            public Exception Exception { get; set; }
            public bool Initialised { get; set; }
            public bool TestOptValue { get; set; }

            [Command("c")]
            public class Command
            {
                [Option]
                public bool TestOpt { get; set; }
            }

            [Command("F")]
            public class FailingCommand
            {
            }

            [Command("T")]
            public class ThrowingCommand
            {
            }

            [Command("TSelf")]
            public class SelfThrowingCommand
            {
                [CommandHandler]
                public void SelfHandler()
                {
                    throw new Exception("Exception from throwing self handling command.");
                }
            }

            [Command("TClass")]
            public class ClassHandledThrowingCommand
            {
                
            }

            [CommandHandler]
            public class ClassHandledThrowingCommandHandler
            {
                public void Handle(IConsoleAdapter console, IErrorAdapter error, ClassHandledThrowingCommand cmd)
                {
                    throw new Exception("Exception from throwing class handled command.");
                }
            }


            [Command("d")]
            public class SelfCommand
            {
                [Positional]
                public string Pos { get; set; }

                [Option]
                public string TestOpt { get; set; }

                [CommandHandler]
                public void Handle(IConsoleAdapter console)
                {
                    console.WrapLine("Self handled command.");
                    console.WrapLine("Positional parameter: {0}", Pos);
                    console.WrapLine("Option TestOpt      : {0}", TestOpt);

                }
            }

            [Command("e")]
            public class ClassHandledCommand
            {
                [Positional]
                public string Pos { get; set; }

                [Option]
                public string TestOpt { get; set; }
            }

            [CommandHandler]
            public class ClassHandledHandler
            {
                public void Handle(IConsoleAdapter console, IErrorAdapter error, ClassHandledCommand cmd)
                {
                    console.WrapLine("Class handled command.");
                    console.WrapLine("Positional parameter: {0}", cmd.Pos);
                    console.WrapLine("Option TestOpt      : {0}", cmd.TestOpt);
                }
            }

            public TestApp()
            {
                LastTestApp = this;
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<TestApp>(args);
            }

            protected override void Initialise()
            {
                Initialised = true;

                SetConfigTypeFilter(t => t.DeclaringType == GetType());
                Toolkit.SetCommandExceptionHandler(ExceptionHandler);
            }

            private void ExceptionHandler(IConsoleAdapter console, IErrorAdapter error, Exception exception, object command)
            {
                Exception = exception;
            }

            public void HandleCommand(Command c)
            {
                TestOptValue = c.TestOpt;
            }

            public void HandleCommand(FailingCommand c)
            {
                Environment.ExitCode = 100;
            }

            public void HandleCommand(ThrowingCommand c)
            {
                throw new Exception("Exception from throwing command.");
            }

            protected override void OnCommandSuccess()
            {
                PostSuccessCalled = true;
                base.OnCommandSuccess();
            }

            protected override void OnCommandFailure()
            {
                PostFailureCalled = true;
                base.OnCommandFailure();
            }

            protected override void OnCommandLineValid(object command)
            {
                LastCommandLineValidObject = command;

                if (_setExitCodeInCommandLineValid)
                    Environment.ExitCode = 1000;

                base.OnCommandLineValid(command);
            }

            public object LastCommandLineValidObject { get; set; }
            public bool PostSuccessCalled { get; set; }
            public bool PostFailureCalled { get; set; }

            public static void SetExitCodeInCommandLineValid()
            {
                _setExitCodeInCommandLineValid = true;
            }

            public static void Reset()
            {
                _setExitCodeInCommandLineValid = false;
            }
        }
        
        public class DuplicateCommandHandlerApp : CommandDrivenApplication
        {
            [Command]
            public class Command
            {
            }

            [Command]
            public class Command2
            {
            }

            [CommandHandler(typeof(Command))]
            public class Handler
            {
                public void Handle(Command c)
                {
                }
            }

            [CommandHandler(typeof(Command2))]
            public class Handler2
            {
                public void Handle(Command2 c)
                {
                }
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<DuplicateCommandHandlerApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public void Command2Handler(Command2 command)
            {}
        }
        
        public class HelpCommandApp : CommandDrivenApplication
        {
            [Command]
            [Description("A test command that does nothing. This text is here just to add help information.")]
            public class Command
            {
            }

            [Command]
            [Description("Get help on the commands supported by this application.")]
            private class HelpMe
            {
                [Positional(0, DefaultValue = null)]
                [Description("Optional command on which help is required.")]
                public string Subject;
            }

            [CommandHandler(typeof(Command))]
            public class Handler
            {
                public void Handle(Command c)
                {
                }
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<HelpCommandApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
                HelpCommand<HelpMe>(h => h.Subject);
            }
        }
        
        public class InvalidHelpCommandApp : CommandDrivenApplication
        {
            [Command]
            public class Command
            {
            }

            private class HelpMe
            {
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<InvalidHelpCommandApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
                HelpCommand<HelpMe>(h => null);
            }
        }

        public class DefaultExceptionHandlerApp : CommandDrivenApplication
        {
            [Command("X")]
            public class Command
            {
                [CommandHandler]
                public void Handle()
                {
                    throw new Exception("Failure exception message");
                }
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<DefaultExceptionHandlerApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }
        #endregion

        public TestCommandDrivenApplication()
        {
            _console = new ConsoleInterfaceForTesting();
            Environment.ExitCode = 0;
        }

        void IDisposable.Dispose()
        {
            Toolkit.GlobalReset();
            Environment.ExitCode = 0;
            TestApp.Reset();
        }

        [Fact]
        public void InitialiseIsCalled()
        {
            UnitTestAppRunner.Run<TestApp>();
            TestApp.LastTestApp.Initialised.Should().BeTrue();
        }

        [Fact]
        public void CommandIsExecuted()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "C", "-TestOpt" }, new RedirectedConsole(ConsoleStream.Out));
            TestApp.LastTestApp.TestOptValue.Should().BeTrue();
        }

        [Fact]
        public void SelfHandledCommandIsExecuted()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "d", "positional", "-TestOpt:opt" }, _console);
            Approvals.Verify(_console.GetBuffer());
        }

        [Fact]
        public void ClassHandledCommandIsExecuted()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "e", "positional", "-TestOpt:opt" }, _console);
            Approvals.Verify(_console.GetBuffer());
        }

        [Fact]
        public void StaticParsingConventionsAreUsed()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppRunner.Run<TestApp>(new [] {"c","/TestOpt"});
            TestApp.LastTestApp.TestOptValue.Should().BeTrue();
        }

        [Fact]
        public void ApplicationWithDuplicateCommandHandlersWillNotInitialise()
        {
            Assert.Throws<MultipleHandlersForCommand>(() => UnitTestAppRunner.Run<DuplicateCommandHandlerApp>(new string[] {}));
        }

        [Fact]
        public void HelpIsProvidedWithIndicatedCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            _console.Write("helpme:" + Environment.NewLine + Environment.NewLine);
            UnitTestAppRunner.Run<HelpCommandApp>(new[] { "helpme" }, _console);
            _console.Write(Environment.NewLine + Environment.NewLine);

            _console.Write("helpme command:" + Environment.NewLine + Environment.NewLine);
            UnitTestAppRunner.Run<HelpCommandApp>(new[] { "helpme", "command" }, _console);
            _console.Write(Environment.NewLine + Environment.NewLine);

            _console.Write("helpme helpme:" + Environment.NewLine + Environment.NewLine);
            UnitTestAppRunner.Run<HelpCommandApp>(new[] { "helpme", "helpme" }, _console);

            Approvals.Verify(_console.GetBuffer());
        }

        [Fact]
        public void CommandLevelHelpIsProvidedWithIndicatedCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppRunner.Run<HelpCommandApp>(new[] { "helpme", "helpme" }, _console);
            Approvals.Verify(_console.GetBuffer());
        }

        [Fact]
        public void HelpCommandTypeMustBeAConfiguredCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            Assert.Throws<HelpCommandMustBePartOfConfiguration>(() => UnitTestAppRunner.Run<InvalidHelpCommandApp>(new[] { "helpme" }, _console));
        }

        [Fact]
        public void OnCommandSuccessIsCalledAfterSuccessfulCommand()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "C", "-TestOpt" }, new RedirectedConsole(ConsoleStream.Out));

            //Assert
            TestApp.LastTestApp.PostSuccessCalled.Should().BeTrue();
        }

        [Fact]
        public void OnCommandFailureIsNotCalledAfterSuccessfulCommand()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "C", "-TestOpt" }, new RedirectedConsole(ConsoleStream.Out));

            //Assert
            TestApp.LastTestApp.PostFailureCalled.Should().BeFalse();
        }

        [Fact]
        public void OnCommandFailureIsCalledAfterFailedCommand()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "F" }, new RedirectedConsole(ConsoleStream.Out));

            //Assert
            TestApp.LastTestApp.PostFailureCalled.Should().BeTrue();
        }

        [Fact]
        public void OnCommandSuccessIsNotCalledAfterFailedCommand()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "F" }, new RedirectedConsole(ConsoleStream.Out));

            //Assert
            TestApp.LastTestApp.PostSuccessCalled.Should().BeFalse();
        }

        [Fact]
        public void OnCommandSuccessIsNotCalledAfterThrowingCommand()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "T" }, new RedirectedConsole(ConsoleStream.Out));

            //Assert
            TestApp.LastTestApp.PostSuccessCalled.Should().BeFalse();
        }

        [Fact]
        public void ExceptionHandlerIsCalledWhenCommandThrows()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "T" }, new RedirectedConsole(ConsoleStream.Out));

            //Assert
            Assert.Equal("Exception from throwing command.", TestApp.LastTestApp.Exception.Message);
        }

        [Fact]
        public void ExceptionHandlerIsCalledWhenSelfHandlerCommandThrows()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "TSelf" }, new RedirectedConsole(ConsoleStream.Out));

            //Assert
            Assert.Equal("Exception from throwing self handling command.", TestApp.LastTestApp.Exception.Message);
        }

        [Fact]
        public void ExceptionHandlerIsCalledWhenClassHandlerThrows()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "TClass" }, new RedirectedConsole(ConsoleStream.Out));

            //Assert
            Assert.Equal("Exception from throwing class handled command.", TestApp.LastTestApp.Exception.Message);
        }

        [Fact]
        public void DefaultExceptionHandlerDisplaysExceptionMessage()
        {
            //Act
            UnitTestAppRunner.Run<DefaultExceptionHandlerApp>(new[] { "X" }, _console);

            //Assert
            Approvals.Verify(_console.GetBuffer());
        }

        [Fact]
        public void CommandLineValidHandlerCalledWithValidCommand()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "c" }, _console);

            //Assert
            TestApp.LastTestApp.LastCommandLineValidObject.Should().BeOfType<TestApp.Command>();
        }

        [Fact]
        public void CommandNotExecutedIfCommandLineValidHandlerSetsNonZeroExitCode()
        {
            //Arrange
            TestApp.SetExitCodeInCommandLineValid();

            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "C", "-TestOpt" }, _console);

            //Assert
            TestApp.LastTestApp.TestOptValue.Should().BeFalse();
        }

        [Fact]
        public void CommandLineValidHandlerNotCalledWhenCommandInvalid()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new[] { "c", "toomany" }, _console);

            //Assert
            TestApp.LastTestApp.LastCommandLineValidObject.Should().BeNull();
        }

        [Fact]
        public void HelpIsDisplayedWhenNoParametersAreSupplied()
        {
            //Act
            UnitTestAppRunner.Run<TestApp>(new string[0], _console);

            //Assert
            Approvals.Verify(_console.GetBuffer());
        }

        [Fact]
        public void HelpIsDisplayedWhenNoParametersAreSuppliedAndHelpIsImplemented()
        {
            //Act
            UnitTestAppRunner.Run<HelpCommandApp>(new string[0], _console);

            //Assert    
            Approvals.Verify(_console.GetBuffer());
        }
    }
}
