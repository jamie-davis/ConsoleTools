using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestReadInputItem
    {
        private ConsoleInterfaceForTesting _interface;
        private ConsoleAdapter _adapter;

        private static readonly PropertyInfo StringProp = typeof (TestReadInputItem).GetProperty("StringVal");
        private static readonly PropertyInfo IntProp = typeof (TestReadInputItem).GetProperty("IntVal");
        private TextReader _goodStream;
        private TextReader _selectStream;
        private TextReader _stringOnlyStream;

        public string StringVal { get; set; }
        public int IntVal { get; set; }

        [SetUp]
        public void SetUp()
        {
            _interface = new ConsoleInterfaceForTesting();
            _adapter = new ConsoleAdapter(_interface);

            StringVal = null;
            IntVal = 0;

            _goodStream = MakeStream(new [] {"text", "45"});
            _stringOnlyStream = MakeStream(new [] {"text"});
            _selectStream = MakeStream(new [] {"bad", "2", "C"});
        }

        private TextReader MakeStream(IEnumerable<string> input)
        {
            return new StringReader(input.JoinWith(Environment.NewLine));
        }

        [Test]
        public void StringCanBeRead()
        {
            _interface.SetInputStream(_goodStream);
            var item = new InputItem<string>
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string)
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Assert.That(item.Value, Is.EqualTo("text"));
        }

        [Test]
        public void ValidReadReturnsTrue()
        {
            _interface.SetInputStream(_stringOnlyStream);
            var item = new InputItem<string>
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string)
            };

            Assert.That(ReadInputItem.GetValue(item, _interface, _adapter), Is.True);
        }

        [Test]
        public void InvalidReadReturnsFalse()
        {
            _interface.SetInputStream(_stringOnlyStream);
            _interface.InputIsRedirected = true;
            var item = new InputItem<Read<string>>
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            Assert.That(ReadInputItem.GetValue(item, _interface, _adapter), Is.False);
        }

        [Test]
        public void TextIsConvertedToRequiredType()
        {
            _interface.SetInputStream(_goodStream);
            var item = new InputItem<Read<int>>
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Assert.That(item.Value, Is.EqualTo(45));
        }

        [Test]
        public void PromptIsDisplayed()
        {
            _interface.SetInputStream(_goodStream);
            var item = new InputItem<Read<string>>
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string),
                ReadInfo = Read.String().Prompt("prompt")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void ErrorIsDisplayedWhenInputIsInvalid()
        {
            _interface.SetInputStream(_goodStream);
            _interface.InputIsRedirected = true;
            var item = new InputItem<Read<int>>
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void InteractiveInputContinuesUntilGoodInputReceived()
        {
            _interface.SetInputStream(_goodStream);
            var item = new InputItem<Read<int>>
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void OptionsCanBeSpecified()
        {
            _interface.SetInputStream(_selectStream);
            var item = new InputItem<Read<int>>
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                .Option(1, "1", "First")
                .Option(2, "2", "Second")
                .Option(3, "3", "Third")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void OptionIsSelected()
        {
            _interface.SetInputStream(_selectStream);
            var item = new InputItem<Read<int>>
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                .Option(100, "B", "First")
                .Option(200, "C", "Second")
                .Option(300, "D", "Third")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            var value = ((Read<int>)item.Value).Value;
            Assert.That(value, Is.EqualTo(200));
        }
    }
}