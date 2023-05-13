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

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ConsoleToolkitTests.ApplicationStyles
{
    [UseReporter(typeof(CustomReporter))]
    public class TestConsoleApplication : IDisposable
    {
        private ConsoleInterfaceForTesting _consoleOut;

        #region Types for test

        public class TestApp : ConsoleApplication
        {
            private static bool _setExitCodeInCommandLineValid
                ;
            public static TestApp LastTestApp { get; set; }
            public bool Initialised { get; set; }
            public bool TestOptValue { get; set; }
            public Exception LastException { get; private set; }
            public object ValidatedCommandObject { get; set; }
            public bool OnCommandLineValidCalled { get; set; }
            public Command HandledCommandObject { get; set; }

            [Command]
            public class Command
            {
                [Option]
                public bool TestOpt { get; set; }

                [Option]
                public bool Fail { get; set; }

                [Option]
                public bool Throw { get; set; }
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

            protected override void OnCommandLineValid(object command)
            {
                OnCommandLineValidCalled = true;
                ValidatedCommandObject = command;

                if (_setExitCodeInCommandLineValid)
                    Environment.ExitCode = 1000;

                base.OnCommandLineValid(command);
            }

            private void ExceptionHandler(IConsoleAdapter console, IErrorAdapter error, Exception exception, object options)
            {
                LastException = exception;
            }

            public void HandleCommand(Command c)
            {
                TestOptValue = c.TestOpt;
                if (c.Fail)
                    Environment.ExitCode = 100;

                if (c.Throw)
                    throw new Exception("TestApp exception.");

                HandledCommandObject = c;
            }

            protected override void OnCommandSuccess()
            {
                CommandSuccessCalled = true;
                base.OnCommandSuccess();
            }

            protected override void OnCommandFailure()
            {
                CommandFailureCalled = true;
                base.OnCommandFailure();
            }

            public bool CommandSuccessCalled { get; set; }
            public bool CommandFailureCalled { get; set; }

            public static void SetExitCodeInCommandLineValid()
            {
                _setExitCodeInCommandLineValid = true;
            }

            public static void Reset()
            {
                _setExitCodeInCommandLineValid = false;
            }
        }

        public class MultipleCommandHandlerApp : ConsoleApplication
        {
            [Command]
            public class Command
            {
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
                Toolkit.Execute<MultipleCommandHandlerApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public void CommandHandler(Command command)
            { }
        }

        public class MultipleCommandApp : ConsoleApplication
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

            public static void XMain(string[] args)
            {
                Toolkit.Execute<MultipleCommandApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public void Command2Handler(Command2 command)
            { }
        }

        public class InteractiveCommandApp : ConsoleApplication
        {
            [Command]
            public class Command
            {
            }

            [InteractiveCommand]
            public class Command2
            {
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<MultipleCommandApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public void Handler(Command c)
            {
                TestOptValue = true;
            }

            public static bool TestOptValue { get; set; }
        }

        public class HelpApp : ConsoleApplication
        {
            [Command]
            [Description("A test application that only exists to check that this text is displayed automatically as part of the help information.")]
            public class Command
            {
                [Option("h", ShortCircuit = true)]
                [Description("Display this help text.")]
                public bool Help { get; set; }
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
                Toolkit.Execute<HelpApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
                HelpOption<Command>(c => c.Help);
            }
        }

        public class InvalidHelpApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
                
            }

            public class Command
            {
                public bool Help { get; set; }
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<InvalidHelpApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
                HelpOption<Command>(c => c.Help);
            }
        }

        public class SelfHandledCommandApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
                [CommandHandler]
                public void Handle(IConsoleAdapter console)
                {
                    console.Write("output");
                }
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<SelfHandledCommandApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }

        public class HandlerClassApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
            }

            [CommandHandler(typeof(Options))]
            public class HandlerClass
            {
                public void Handle(IConsoleAdapter console, Options command, IErrorAdapter error)
                {
                    console.WriteLine("Text from handler class.");
                    error.WriteLine("Error text");
                }
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<HandlerClassApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }

        public class CustomInjectionApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
            }

            public class CustomObject
            {
                public string Message { get; set; }
            }

            [CommandHandler(typeof(Options))]
            public class HandlerClass
            {
                public void Handle(IConsoleAdapter console, IErrorAdapter error, Options command, CustomObject custom)
                {
                    console.WriteLine("Custom string is \"{0}\"", custom.Message);
                }
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<CustomInjectionApp>(args);
            }

            protected override void Initialise()
            {
                RegisterInjectionInstance(new CustomObject { Message = "Custom message" });
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }

        public class AutoHelpApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
                [Positional]
                public string Pos { get; set; }
            }

            public class CustomObject
            {
                public string Message { get; set; }
            }

            [CommandHandler(typeof(Options))]
            public class HandlerClass
            {
                public void Handle(IConsoleAdapter console, IErrorAdapter error, Options command)
                {
                    console.WriteLine("Parameter is \"{0}\"", command.Pos);
                }
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<CustomInjectionApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }
        #endregion

        public TestConsoleApplication()
        {
            _consoleOut = new ConsoleInterfaceForTesting();
            Environment.ExitCode = 0;
        }

        void IDisposable.Dispose()
        {
            Toolkit.GlobalReset();
            TestApp.Reset();
            Environment.ExitCode = 0;
        }

        [Fact]
        public void InitialiseIsCalled()
        {
            UnitTestAppRunner.Run<TestApp>();
            TestApp.LastTestApp.Initialised.Should().BeTrue();
        }

        [Fact]
        public void OnCommandLineValidIsCalled()
        {
            UnitTestAppRunner.Run<TestApp>();
            TestApp.LastTestApp.OnCommandLineValidCalled.Should().BeTrue();
        }

        [Fact]
        public void OnCommandLineValidReceivesCommand()
        {
            UnitTestAppRunner.Run<TestApp>();
            TestApp.LastTestApp.ValidatedCommandObject.Should().BeSameAs(TestApp.LastTestApp.HandledCommandObject);
        }

        [Fact]
        public void CommandIsNotExecutedIfOnCommandLineValidSetsExitCode()
        {
            TestApp.SetExitCodeInCommandLineValid();
            UnitTestAppRunner.Run<TestApp>();
            TestApp.LastTestApp.HandledCommandObject.Should().BeNull();
        }

        [Fact]
        public void CommandIsExecuted()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "-TestOpt" }, new RedirectedConsole(ConsoleStream.Out));
            TestApp.LastTestApp.TestOptValue.Should().BeTrue();
        }

        [Fact]
        public void AppWithInteractiveCommandsCanBeExecuted()
        {
            UnitTestAppRunner.Run<InteractiveCommandApp>(new string[] {}, new RedirectedConsole(ConsoleStream.Out));
            InteractiveCommandApp.TestOptValue.Should().BeTrue();
        }

        [Fact]
        public void StaticParsingConventionsAreUsed()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppRunner.Run<TestApp>(new[] { "/TestOpt" });
            TestApp.LastTestApp.TestOptValue.Should().BeTrue();
        }

        [Fact]
        public void ApplicationWithMultipleCommandHandlersWillNotInitialise()
        {
            Assert.Throws<MultipleHandlersForCommand>(() => UnitTestAppRunner.Run<MultipleCommandHandlerApp>(new string[] { }));
        }

        [Fact]
        public void ApplicationWithMultipleCommandsWillNotInitialise()
        {
            Assert.Throws<MultipleCommandsInvalid>(() => UnitTestAppRunner.Run<MultipleCommandApp>(new string[] { }));
        }

        [Fact]
        public void HelpIsProvidedWithIndicatedCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppRunner.Run<HelpApp>(new[] { "/h" }, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Fact]
        public void HelpCommandTypeMustBeAConfiguredCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            Assert.Throws<HelpCommandMustBePartOfConfiguration>(() => UnitTestAppRunner.Run<InvalidHelpApp>(new[] { "/h" }, _consoleOut));
        }

        [Fact]
        public void SelfHandledCommandIsExecuted()
        {
            UnitTestAppRunner.Run<SelfHandledCommandApp>(new string[] {}, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Fact]
        public void HandlerClassCommandIsExecuted()
        {
            UnitTestAppRunner.Run<HandlerClassApp>(new string[] {}, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Fact]
        public void CustomInstanceCanBeInjectedIntoHandler()
        {
            UnitTestAppRunner.Run<CustomInjectionApp>(new string[] {}, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Fact]
        public void OnCommandSuccessIsCalledAfterSuccessfulRun()
        {
            UnitTestAppRunner.Run<TestApp>(new string[] {}, _consoleOut);
            TestApp.LastTestApp.CommandSuccessCalled.Should().BeTrue();
        }

        [Fact]
        public void OnCommandSuccessIsNotCalledAfterFailedRun()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "-Fail" }, _consoleOut);
            TestApp.LastTestApp.CommandSuccessCalled.Should().BeFalse();
        }

        [Fact]
        public void OnCommandFailureIsCalledAfterFailedRun()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "-Fail" }, _consoleOut);
            Console.WriteLine(_consoleOut.GetBuffer());
            TestApp.LastTestApp.CommandFailureCalled.Should().BeTrue();
        }

        [Fact]
        public void OnCommandFailureIsCalledAfterExceptionIsThrown()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "-Throw" }, _consoleOut);
            Console.WriteLine(_consoleOut.GetBuffer());
            TestApp.LastTestApp.CommandFailureCalled.Should().BeTrue();
        }

        [Fact]
        public void OnCommandFailureIsNotCalledAfterSuccesfulRun()
        {
            UnitTestAppRunner.Run<TestApp>(new string[] {}, _consoleOut);
            TestApp.LastTestApp.CommandFailureCalled.Should().BeFalse();
        }

        [Fact]
        public void ExceptionHandlerIsCalledWhenExceptionIsThrownByCommandHandler()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "-Throw" }, _consoleOut);
            Console.WriteLine(_consoleOut.GetBuffer());
            Assert.Equal("TestApp exception.", TestApp.LastTestApp.LastException.Message);
        }

        [Fact]
        public void NoParametersGivesHelpText()
        {
            UnitTestAppRunner.Run<AutoHelpApp>(new string[0], _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Fact]
        public void NoParametersHelpSetsExitCode()
        {
            UnitTestAppRunner.Run<AutoHelpApp>(new string[0], _consoleOut);
            Environment.ExitCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public void NoHelpTextIfNoParametersIsValid()
        {
            UnitTestAppRunner.Run<HandlerClassApp>(new string[0], _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

    }
}