using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.Annotations;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Exceptions;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestReadInputImpl
    {
        private ConsoleInterfaceForTesting _interface;
        private StringReader _intStringData;
        private ConsoleAdapter _adapter;

        #region Types for test

        class IntString
        {
            public int Int { [UsedImplicitly] get; set; }
            public string String { [UsedImplicitly] get; set; }
        }

        class Constructable
        {
            public Read<string> String { [UsedImplicitly] get; set; }

            public Constructable()
            {
                String = new Read<string>().Prompt("string prompt: ");
            }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _interface = new ConsoleInterfaceForTesting();
            _adapter = new ConsoleAdapter(_interface);

            _intStringData = new StringReader(@"45
some text");
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var readerField in GetType().GetFields(BindingFlags.NonPublic).Where(t => t.FieldType == typeof(TextReader)))
            {
                var reader = readerField.GetValue(this) as StringReader;
                if (reader != null)
                    reader.Dispose();
            }
        }

        private TextReader MakeStream(IEnumerable<string> input)
        {
            return new StringReader(input.JoinWith(Environment.NewLine));
        }

        private ReadInputImpl<T> GetImplWithoutTemplate<T>(T template) where T : class
        {
            return new ReadInputImpl<T>(_interface, _adapter);
        }

        private ReadInputImpl<T> GetImplByTemplate<T>(T template) where T : class
        {
            return new ReadInputImpl<T>(_interface, _adapter, template);
        }

        private ReadInputImpl<Read<T>> GetImplForRead<T>(Read<T> read)
        {
            return new ReadInputImpl<Read<T>>(_interface, _adapter, read);
        }


        [Test]
        public void ClassPropertiesAreIdentified()
        {
            _interface.SetInputStream(_intStringData);
            var impl = new ReadInputImpl<IntString>(_interface, _adapter);
            var props = impl.Properties.Select(p => p.TypeDescription()).JoinWith(",");
            Assert.That(props, Is.EqualTo("Int32 Int,String String"));
        }

        [Test]
        public void ClassPropertiesAreReadFromStream()
        {
            _interface.SetInputStream(_intStringData);
            var impl = new ReadInputImpl<IntString>(_interface, _adapter);
            var result = impl.Result.WritePropertiesToString();
            var expected = new IntString { Int = 45, String = "some text" }.WritePropertiesToString();
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TuplePropertiesAreIdentified()
        {
            _interface.SetInputStream(_intStringData);
            var impl = new ReadInputImpl<Tuple<int, string>>(_interface, _adapter);
            var props = impl.Properties.Select(p => p.TypeDescription()).JoinWith(",");
            Assert.That(props, Is.EqualTo("Int32 Item1,String Item2"));
        }

        [Test]
        public void TuplePropertiesAreReadFromStream()
        {
            _interface.SetInputStream(_intStringData);
            var impl = new ReadInputImpl<Tuple<int,string>>(_interface, _adapter);
            var result = impl.Result.WritePropertiesToString();
            var expected = new Tuple<int, string>(45, "some text").WritePropertiesToString();
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AnonymousTypePropertiesAreIdentified()
        {
            _interface.SetInputStream(_intStringData);
            var impl = GetImplByTemplate(new {Int = 4509, String = "ddd"});
            var props = impl.Properties.Select(p => p.TypeDescription()).JoinWith(",");
            Assert.That(props, Is.EqualTo("Int32 Int,String String"));
        }

        [Test]
        public void AnonymousTypePropertiesAreReadFromStream()
        {
            _interface.SetInputStream(_intStringData);
            var impl = GetImplByTemplate(new { Int = 4509, String = "ddd" });
            var result = impl.Result.WritePropertiesToString();
            var expected = new { Int = 45, String = "some text" }.WritePropertiesToString();
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CustomReadMembersAreReadFromStream()
        {
            _interface.SetInputStream(_intStringData);
            var impl = GetImplByTemplate(new {Int = Read.Int().Prompt("age"), String = Read.String()});
            var props = impl.Properties.Select(p => p.TypeDescription()).JoinWith(",");
            Assert.That(props, Is.EqualTo("Int32 Int,String String"));
        }

        [Test]
        public void CustomReadMemberInfoIsLoadedFromTemplate()
        {
            _interface.SetInputStream(_intStringData);
            var impl = GetImplByTemplate(new {Int = Read.Int().Prompt("age"), String = Read.String()});
            var prop = impl.Properties.FirstOrDefault(p => p.Name == "Int");
            Debug.Assert(prop != null, "prop != null");

            Assert.That(prop.ReadInfo.Prompt, Is.EqualTo("age"));
        }

        [Test, ExpectedException(typeof(ReadPropertyInvalidWithoutTemplate))]
        public void AnExceptionIsThrownIfReadPropertiesAreUsedWithoutATemplate()
        {
            _interface.SetInputStream(_intStringData);
            var template = new {Int = Read.Int().Prompt("age"), String = Read.String()};
            GetImplWithoutTemplate(template);
        }

        [Test]
        public void ReadPropertiesCanBeUsedWithoutATemplateIfTheTypeHasADefaultConstructor()
        {
            _interface.SetInputStream(_intStringData);
            var impl = new ReadInputImpl<Constructable>(_interface, _adapter).Result;
        }

        [Test, ExpectedException(typeof(ReadPropertyMustBeInitialised))]
        public void AnExceptionIsThrownIfReadPropertiesAreNullInTemplate()
        {
            _interface.SetInputStream(_intStringData);
            var template = new { Int = (Read<int>)null };
            GetImplByTemplate(template);
        }

        [Test]
        public void CustomReadPropertiesAreLoadedFromStream()
        {
            _interface.SetInputStream(_intStringData);
            var impl = GetImplByTemplate(new {Int = Read.Int().Prompt("age"), String = Read.String()});
            var instance = string.Format("Int = {0}, String = {1}", impl.Result.Int.Value, impl.Result.String.Value);
            const string expected = "Int = 45, String = some text";

            Assert.That(instance, Is.EqualTo(expected));
        }

        [Test]
        public void PromptsAreDisplayedIfInputIsNotRedirected()
        {
            _interface.SetInputStream(_intStringData);
            _interface.InputIsRedirected = false;

            var template = new
            {
                Int = Read.Int().Prompt("age: "),
                String = Read.String()
            };

            GetImplByTemplate(template);

            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void PromptsAreDisplayedIfInputIsRedirected()
        {
            _interface.SetInputStream(_intStringData);
            _interface.InputIsRedirected = true;

            var template = new
            {
                Int = Read.Int().Prompt("age: "),
                String = Read.String()
            };

            GetImplByTemplate(template);

            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void ErrorsAreDisplayedWhenTheInputIsInvalid()
        {
            var input = new[]
            {
                "invalid"
            };

            _interface.SetInputStream(MakeStream(input));
            _interface.InputIsRedirected = true;

            var template = new
            {
                Int = Read.Int(),
                String = Read.String()
            };

            GetImplByTemplate(template);

            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void BadValuesMustBeReInput()
        {
            var input = new[]
            {
                "invalid",
                "still bad",
                "45",
                "string"
            };

            _interface.SetInputStream(MakeStream(input));
            _interface.InputIsRedirected = false;

            var template = new
            {
                Int = Read.Int(),
                String = Read.String()
            };

            GetImplByTemplate(template);

            Approvals.Verify(_interface.GetBuffer());
        }

        [Test]
        public void PopulatedObjectIsReturnedAfterBadInputIsCorrected()
        {
            var input = new[]
            {
                "invalid",
                "still bad",
                "45",
                "string"
            };

            _interface.SetInputStream(MakeStream(input));
            _interface.InputIsRedirected = false;

            var template = new
            {
                Int = Read.Int(),
                String = Read.String()
            };

            var impl = GetImplByTemplate(template);
            var instance = string.Format("Int = {0}, String = {1}", impl.Result.Int.Value, impl.Result.String.Value);
            const string expected = "Int = 45, String = string";

            Assert.That(instance, Is.EqualTo(expected));
        }

        [Test]
        public void ReadObjectsCanBeInputDirectly()
        {
            var input = new[]
            {
                "invalid",
                "still bad",
                "45"
            };

            _interface.SetInputStream(MakeStream(input));
            _interface.InputIsRedirected = false;

            var readObject = Read.Int().Prompt("Enter a number: ");
            var impl = GetImplForRead(readObject);

            Assert.That(impl.Result.Value, Is.EqualTo(45));
        }

        [Test]
        public void DirectReadObjectsDisplayPrompt()
        {
            var input = new[]
            {
                "invalid",
                "still bad",
                "45"
            };

            _interface.SetInputStream(MakeStream(input));
            _interface.InputIsRedirected = false;

            var readObject = Read.Int().Prompt("Enter a number");
            var impl = GetImplForRead(readObject);

            Approvals.Verify(_interface.GetBuffer());
        }
    }
}
