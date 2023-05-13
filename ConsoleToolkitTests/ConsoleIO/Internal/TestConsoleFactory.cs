using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    public class TestConsoleFactory
    {
        [Fact]
        public void DefaultConsoleIsCreatedIfConsoleNotRedirected()
        {
            //Arrange
            var factory = new ConsoleFactory(new FakeConsoleRedirectTester(false, false));

            //Act
            var console = factory.Console;

            //Assert
            console.Should().BeOfType<DefaultConsole>();
        }

        [Fact]
        public void RedirectedConsoleIsCreatedIfConsoleIsRedirected()
        {
            //Arrange
            var factory = new ConsoleFactory(new FakeConsoleRedirectTester(true, false));

            //Act
            var console = factory.Console;

            //Assert
            console.Should().BeOfType<RedirectedConsole>();
        }
        [Fact]
        public void DefaultConsoleIsCreatedForErrorIfErrorNotRedirected()
        {
            //Arrange
            var factory = new ConsoleFactory(new FakeConsoleRedirectTester(false, false));

            //Act
            var error = factory.Error;

            //Assert
            error.Should().BeOfType<DefaultConsole>();
        }

        [Fact]
        public void RedirectedConsoleIsCreatedForErrorIfErrorIsRedirected()
        {
            //Arrange
            var factory = new ConsoleFactory(new FakeConsoleRedirectTester(false, true));

            //Act
            var error = factory.Error;

            //Assert
            error.Should().BeOfType<RedirectedConsole>();
        }

        private class FakeConsoleRedirectTester : IConsoleRedirectTester
        {
            private readonly bool _consoleRedirected;
            private readonly bool _errorRedirected;

            public FakeConsoleRedirectTester(bool consoleRedirected, bool errorRedirected)
            {
                _consoleRedirected = consoleRedirected;
                _errorRedirected = errorRedirected;
            }

            public bool IsOutputRedirected()
            {
                return _consoleRedirected;
            }

            public bool IsErrorRedirected()
            {
                return _errorRedirected;
            }
        }
    }
}
