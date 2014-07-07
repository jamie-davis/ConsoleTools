using System;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkitTests.TestingUtilities;
using Microsoft.SqlServer.Server;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestParameterAssignmentGenerator
    {

        #region Types for test
        
        //Don't complain about unused things
        #pragma warning disable 649 
        #pragma warning disable 169
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable MemberCanBePrivate.Local

        private class TestType
        {
            public int IntProp { get; set; }
            public int IntField;
            public string StringProp { get; set; }
            public string StringField;
            public Nested NestedProp { get; set; }

            public class Nested
            {
                public int IntField;
                public string StringField;

                public override string ToString()
                {
                    return string.Format("{0}, {1}", IntField, StringField);
                }
            }
        }

        private class TestTypeFixedOrderNested
        {
            public Nested NestedProp { get; set; }

            public class Nested
            {
                [Positional(3)]
                public int IntProp { get; set; }

                [Positional(2)]
                public string StringProp { get; set; }

                [Positional(0)]
                public double DoubleField;
            }
        }

        private class TestTypeMixedNested
        {
            public Nested NestedProp { get; set; }

            public class Nested
            {
                public int IntProp { get; set; }
                public string StringField;
            }
        }

        private class TestTypeBadNewNested
        {
            public Nested NestedProp { get; set; }

            public class Nested
            {
                public int IntProp { get; set; }
                public string StringProp { get; set; }

                public Nested(int n)
                {
                    
                }
            }
        }

        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore ClassNeverInstantiated.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        #pragma warning restore 169
        #pragma warning restore 649
        #endregion

        [Test]
        public void IntPropAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("IntProp");
            var assignment = (Action<TestType, int>)ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, 10);
            Assert.That(item.IntProp, Is.EqualTo(10));
        }

        [Test]
        public void IntFieldAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var fieldInfo = typeof(TestType).GetField("IntField");
            var assignment = (Action<TestType, int>)ParameterAssignmentGenerator<TestType>.Generate(fieldInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, 10);
            Assert.That(item.IntField, Is.EqualTo(10));
        }

        [Test]
        public void IntAssignmentGenerationReturnsIntParameterType()
        {
            Type[] parameterTypes;
            var fieldInfo = typeof(TestType).GetField("IntField");
            ParameterAssignmentGenerator<TestType>.Generate(fieldInfo, out parameterTypes);
            Assert.That(parameterTypes[0], Is.EqualTo(typeof(int)));
        }

        [Test]
        public void IntAssignmentGenerationReturnsSingleParameterType()
        {
            Type[] parameterTypes;
            var fieldInfo = typeof(TestType).GetField("IntField");
            ParameterAssignmentGenerator<TestType>.Generate(fieldInfo, out parameterTypes);
            Assert.That(parameterTypes.Length, Is.EqualTo(1));
        }

        [Test]
        public void StringPropAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("StringProp");
            var assignment = (Action<TestType, string>)ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, "string value");
            Assert.That(item.StringProp, Is.EqualTo("string value"));
        }

        [Test]
        public void StringFieldAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var fieldInfo = typeof(TestType).GetField("StringField");
            var assignment = (Action<TestType, string>)ParameterAssignmentGenerator<TestType>.Generate(fieldInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, "string value");
            Assert.That(item.StringField, Is.EqualTo("string value"));
        }

        [Test]
        public void NestedClassAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("NestedProp");
            var assignment = (Action<TestType, int, string>)ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, 10, "string value");
            Assert.That(item.NestedProp.ToString(), Is.EqualTo("10, string value"));
        }

        [Test, ExpectedException(typeof(NestedOptionTypeInvalid))]
        public void MixedNestedFieldsAndPropertiesAreInvalid()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestTypeMixedNested).GetProperty("NestedProp");
            ParameterAssignmentGenerator<TestTypeMixedNested>.Generate(propertyInfo, out parameterTypes);
        }

        [Test, ExpectedException(typeof(NestedOptionTypeInvalid))]
        public void NestedClassMustHaveDefaultConstructor()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestTypeBadNewNested).GetProperty("NestedProp");
            ParameterAssignmentGenerator<TestTypeBadNewNested>.Generate(propertyInfo, out parameterTypes);
        }

        [Test]
        public void NumberOfParameterTypesCorrectInNestedClassGeneration()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("NestedProp");
            ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            Assert.That(parameterTypes.Length, Is.EqualTo(2));
        }

        [Test]
        public void PositionalAttributesDetermineParameterOrder()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestTypeFixedOrderNested).GetProperty("NestedProp");
            ParameterAssignmentGenerator<TestTypeFixedOrderNested>.Generate(propertyInfo, out parameterTypes);
            Assert.That(parameterTypes, Is.EqualTo(new []{ typeof(double), typeof(string), typeof(int)}));
        }

        [Test]
        public void ParameterTypesCorrectInNestedClassGeneration()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("NestedProp");
            ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            Assert.That(parameterTypes, Is.EqualTo(new [] {typeof(int), typeof(string)}));
        }
    }
}
