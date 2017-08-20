using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestCommandSelector
    {
        #region Types for test

        class FakeCommand : ICommandKeys
        {
            private List<string> _keywords;
            private string _name;

            #region Implementation of ICommandKeys

            public List<string> Keywords
            {
                get { return _keywords; }
            }

            public string Name
            {
                get { return _name; }
            }

            #endregion

            public FakeCommand(params string[] keys)
            {
                if (keys.Length == 0)
                    throw new Exception("No keys. Invalid.");
                _keywords = keys.Take(keys.Length - 1).ToList();
                _name = keys.Last();
            }
        }
            
        #endregion

        [Test]
        public void SingleCommandWithNoKeywordsIsMatched()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("do") };
            var args = new[] {"do"};

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Matched, Is.True);
        }

        [Test]
        public void IncorrectCommandIsNotMatched()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("go") };
            var args = new[] {"do"};

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Matched, Is.False);
        }

        [Test]
        public void CommandWithKeywordIsMatched()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("operation", "go") };
            var args = new[] {"operation", "go"};

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Matched, Is.True);
        }

        [Test]
        public void CommandWithMultipleKeywordsIsMatched()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("operation", "lets", "go") };
            var args = new[] {"operation", "lets", "go"};

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Matched, Is.True);
        }

        [Test]
        public void CommandWithWrongKeywordIsNotMatched()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("operation", "lets", "go") };
            var args = new[] {"operation", "let", "go"};

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Matched, Is.False);
        }

        [Test]
        public void CommandIsSelectedFromList()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("operation", "lets", "go"), new FakeCommand("config", "add") };
            var args = new[] {"config", "add"};

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Matched, Is.True);
        }

        [Test]
        public void SelectedCommandIsReturned()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("operation", "lets", "go"), new FakeCommand("config", "add") };
            var args = new[] {"config", "add"};

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Command, Is.SameAs(commands[1]));
        }

        [Test]
        public void SelectedCommandIsNullIsNotMatched()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("operation", "lets", "go"), new FakeCommand("config", "add") };
            var args = new[] {"config", "delete"};

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Command, Is.Null);
        }

        [Test]
        public void NumberOfUsedArgumentsIsReturned()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("operation", "lets", "go"), new FakeCommand("config", "add") };
            var args = new[] { "config", "add" };

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.UsedArgumentCount, Is.EqualTo(2));
        }

        [Test]
        public void NumberOfUsedArgumentsIsOneIfNoKeywords()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("go"), new FakeCommand("config", "add") };
            var args = new[] { "go" };

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.UsedArgumentCount, Is.EqualTo(1));
        }

        [Test]
        public void NumberOfUsedArgumentsIsZeroIfNoMatch()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("go"), new FakeCommand("config", "add") };
            var args = new[] { "stop" };

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.UsedArgumentCount, Is.EqualTo(0));
        }

        [Test]
        public void NoMatchesGivesError()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("go"), new FakeCommand("config", "add") };
            var args = new[] { "stop" };

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Error, Is.EqualTo("Command not recognised."));
        }

        [Test]
        public void PartMatchListsContinuations()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("config", "add"), new FakeCommand("get"), new FakeCommand("config", "delete"), new FakeCommand("config", "get") };
            var args = new[] { "config" };

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Error, Is.EqualTo("config must be followed by add, delete or get."));
        }

        [Test]
        public void PartMatchListsSingleContinuation()
        {
            //Arrange
            var commands = new List<ICommandKeys> { new FakeCommand("config", "add"), new FakeCommand("get") };
            var args = new[] { "config" };

            //Act
            var result = CommandSelector.Select(args, commands);

            //Assert
            Assert.That(result.Error, Is.EqualTo("config must be followed by add."));
        }

        [Test]
        public void FindPartialMatchesContainsAllPartialMatches()
        {
            //Arrange
            var output = new UnitTestConsole();
            var commands = new List<ICommandKeys> { new FakeCommand("config", "add"), new FakeCommand("get"), new FakeCommand("config", "delete"), new FakeCommand("config", "get") };
            var args = new[] { "config" };

            //Act
            var result = CommandSelector.FindPartialMatches(args, commands);

            //Assert
            output.Console.WrapLine($"Args = {string.Join(" ", args)}");
            output.Console.WriteLine();
            output.Console.WrapLine(string.Join(Environment.NewLine, result.Select(r => string.Join(" ", r.Keywords.Concat(new [] { r.Name })))));
            Approvals.Verify(output.Interface.GetBuffer());
        }

        [Test]
        public void FindPartialMatchesReturnsExactMatchAlone()
        {
            //Arrange
            var output = new UnitTestConsole();
            var commands = new List<ICommandKeys> { new FakeCommand("config", "add"), new FakeCommand("get"), new FakeCommand("config", "get", "extra"), new FakeCommand("config", "get") };
            var args = new[] { "config", "get" };

            //Act
            var result = CommandSelector.FindPartialMatches(args, commands);

            //Assert
            output.Console.WrapLine($"Args = {string.Join(" ", args)}");
            output.Console.WriteLine();
            output.Console.WrapLine(string.Join(Environment.NewLine, result.Select(r => string.Join(" ", r.Keywords.Concat(new [] { r.Name })))));
            Approvals.Verify(output.Interface.GetBuffer());
        }
    }
}
