using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Testing;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestReadValue
    {
        private ConsoleInterfaceForTesting _interface;
        private ConsoleAdapter _adapter;

        private static readonly PropertyInfo StringProp = typeof(TestReadInputItem).GetProperty("StringVal");
        private static readonly PropertyInfo IntProp = typeof(TestReadInputItem).GetProperty("IntVal");

        public string StringVal { get; set; }
        public int IntVal { get; set; }

        [SetUp]
        public void SetUp()
        {
            _interface = new ConsoleInterfaceForTesting();
            _adapter = new ConsoleAdapter(_interface);

            StringVal = null;
            IntVal = 0;
        }

        private TextReader MakeStream(IEnumerable<string> input)
        {
            return new StringReader(input.JoinWith(Environment.NewLine));
        }

        [Test]
        public void StringCanBeRead()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "text" }));
            var item = new InputItem
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string)
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(value, Is.EqualTo("text"));
        }

        [Test]
        public void ValidReadReturnsTrue()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "text" }));
            var item = new InputItem
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string)
            };

            //Act
            object value;
            var result = ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void InvalidReadReturnsFalse()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "text" }));
            _interface.InputIsRedirected = true;
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            //Act
            object value;
            var result = ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TextIsConvertedToRequiredType()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "45" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(value, Is.EqualTo(45));
        }

        [Test]
        public void ErrorIsDisplayedWhenInputIsInvalid()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "text" }));
            _interface.InputIsRedirected = true;
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void OptionIsSelected()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "C" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                    .Option(100, "B", "First")
                    .Option(200, "C", "Second")
                    .Option(300, "D", "Third")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(value, Is.EqualTo(200));
        }

        [Test]
        public void OptionInputIsNotCaseSensitive()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "c" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                    .Option(100, "B", "First")
                    .Option(200, "C", "Second")
                    .Option(300, "D", "Third")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(value, Is.EqualTo(200));
        }

        [Test]
        public void OptionsCanBeDifferentiatedByCase()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "a", "A" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                    .Option(100, "a", "First")
                    .Option(200, "A", "Second")
            };

            //Act
            object value1, value2;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value1);
            ReadValue.UsingReadLine(item, _interface, _adapter, out value2);

            //Assert
            Assert.That(string.Format("Value1 = {0}, Value2 = {1}", value1, value2), Is.EqualTo("Value1 = 100, Value2 = 200"));
        }

        [Test]
        public void ValidationErrorMessageIsDisplayed()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "10" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void ValidationErrorCausesFalseReturn()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "10" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            //Act
            object value;
            var result = ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidationPassReturnsTrue()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "11" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            //Act
            object value;
            var result = ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidationPassSetsValue()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "11" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.That(value, Is.EqualTo(11));
        }
    }
}