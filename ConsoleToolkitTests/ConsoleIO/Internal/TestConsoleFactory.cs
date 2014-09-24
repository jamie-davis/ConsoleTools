using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    public class TestConsoleFactory
    {
        [Test]
        public void DefaultConsoleIsCreatedIfConsoleNotRedirected()
        {
            //Arrange
            var factory = new ConsoleFactory(new FakeConsoleRedirectTester(false, false));

            //Act
            var console = factory.Console;

            //Assert
            Assert.That(console, Is.InstanceOf<DefaultConsole>());
        }

        [Test]
        public void RedirectedConsoleIsCreatedIfConsoleIsRedirected()
        {
            //Arrange
            var factory = new ConsoleFactory(new FakeConsoleRedirectTester(true, false));

            //Act
            var console = factory.Console;

            //Assert
            Assert.That(console, Is.InstanceOf<RedirectedConsole>());
        }
        [Test]
        public void DefaultConsoleIsCreatedForErrorIfErrorNotRedirected()
        {
            //Arrange
            var factory = new ConsoleFactory(new FakeConsoleRedirectTester(false, false));

            //Act
            var error = factory.Error;

            //Assert
            Assert.That(error, Is.InstanceOf<DefaultConsole>());
        }

        [Test]
        public void RedirectedConsoleIsCreatedForErrorIfErrorIsRedirected()
        {
            //Arrange
            var factory = new ConsoleFactory(new FakeConsoleRedirectTester(false, true));

            //Act
            var error = factory.Error;

            //Assert
            Assert.That(error, Is.InstanceOf<RedirectedConsole>());
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
