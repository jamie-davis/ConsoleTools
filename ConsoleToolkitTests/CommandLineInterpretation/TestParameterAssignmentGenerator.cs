using System;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
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
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local

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

        private class NestedMemberAssignment
        {
            public class ComplexOptionType 
            {
                public string P1 { get; set; }
                public int P2 { get; set; }

                public override string ToString()
                {
                    return string.Format("{0},{1}", P1, P2);
                }
            }

            public class NestedType
            {
                public string String1 { get; set; }
                public ComplexOptionType Complex { get; set; }
            }

            public NestedType Nested { get; set; }
        }

        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore ClassNeverInstantiated.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        #pragma warning restore 169
        #pragma warning restore 649
        #endregion

        [Fact]
        public void IntPropAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("IntProp");
            var assignment = (Action<TestType, int>)ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, 10);
            Assert.Equal(10, item.IntProp);
        }

        [Fact]
        public void IntFieldAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var fieldInfo = typeof(TestType).GetField("IntField");
            var assignment = (Action<TestType, int>)ParameterAssignmentGenerator<TestType>.Generate(fieldInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, 10);
            Assert.Equal(10, item.IntField);
        }

        [Fact]
        public void IntAssignmentGenerationReturnsIntParameterType()
        {
            Type[] parameterTypes;
            var fieldInfo = typeof(TestType).GetField("IntField");
            ParameterAssignmentGenerator<TestType>.Generate(fieldInfo, out parameterTypes);
            Assert.Equal(typeof(int), parameterTypes[0]);
        }

        [Fact]
        public void IntAssignmentGenerationReturnsSingleParameterType()
        {
            Type[] parameterTypes;
            var fieldInfo = typeof(TestType).GetField("IntField");
            ParameterAssignmentGenerator<TestType>.Generate(fieldInfo, out parameterTypes);
            Assert.Equal(1, parameterTypes.Length);
        }

        [Fact]
        public void StringPropAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("StringProp");
            var assignment = (Action<TestType, string>)ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, "string value");
            Assert.Equal("string value", item.StringProp);
        }

        [Fact]
        public void StringFieldAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var fieldInfo = typeof(TestType).GetField("StringField");
            var assignment = (Action<TestType, string>)ParameterAssignmentGenerator<TestType>.Generate(fieldInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, "string value");
            Assert.Equal("string value", item.StringField);
        }

        [Fact]
        public void NestedClassAssignmentIsGenerated()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("NestedProp");
            var assignment = (Action<TestType, int, string>)ParameterAssignmentGenerator<TestType>
                .Generate(propertyInfo, out parameterTypes);
            var item = new TestType();
            assignment(item, 10, "string value");
            Assert.Equal("10, string value", item.NestedProp.ToString());
        }

        [Fact]
        public void MixedNestedFieldsAndPropertiesAreInvalid()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestTypeMixedNested).GetProperty("NestedProp");
            Assert.Throws<NestedOptionTypeInvalid>(() => ParameterAssignmentGenerator<TestTypeMixedNested>.Generate(propertyInfo, out parameterTypes));
        }

        [Fact]
        public void NestedClassMustHaveDefaultConstructor()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestTypeBadNewNested).GetProperty("NestedProp");
            Assert.Throws<NestedOptionTypeInvalid>(() => ParameterAssignmentGenerator<TestTypeBadNewNested>.Generate(propertyInfo, out parameterTypes));
        }

        [Fact]
        public void NumberOfParameterTypesCorrectInNestedClassGeneration()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("NestedProp");
            ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            Assert.Equal(2, parameterTypes.Length);
        }

        [Fact]
        public void PositionalAttributesDetermineParameterOrder()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestTypeFixedOrderNested).GetProperty("NestedProp");
            ParameterAssignmentGenerator<TestTypeFixedOrderNested>.Generate(propertyInfo, out parameterTypes);
            Assert.Equal(new[] { typeof(double), typeof(string), typeof(int) }, parameterTypes);
        }

        [Fact]
        public void ParameterTypesCorrectInNestedClassGeneration()
        {
            Type[] parameterTypes;
            var propertyInfo = typeof(TestType).GetProperty("NestedProp");
            ParameterAssignmentGenerator<TestType>.Generate(propertyInfo, out parameterTypes);
            Assert.Equal(new[] { typeof(int), typeof(string) }, parameterTypes);
        }

        [Fact]
        public void AssignmentToMemberOfNestedTypeIsGenerated()
        {
            var parent = typeof (NestedMemberAssignment).GetProperty("Nested");
            var member = typeof (NestedMemberAssignment.NestedType).GetProperty("String1");
            Type[] parameterTypes;
            var fn = (Action<NestedMemberAssignment, string>) ParameterAssignmentGenerator<NestedMemberAssignment>
                    .Generate(member, out parameterTypes, parent);

            var item = new NestedMemberAssignment {Nested = new NestedMemberAssignment.NestedType()};
            fn(item, "value");
            Assert.Equal("value", item.Nested.String1);
        }

        [Fact]
        public void AssignmentToComplexMemberOfNestedTypeIsGenerated()
        {
            var parent = typeof (NestedMemberAssignment).GetProperty("Nested");
            var member = typeof (NestedMemberAssignment.NestedType).GetProperty("Complex");
            Type[] parameterTypes;
            var fn = (Action<NestedMemberAssignment, string, int>) ParameterAssignmentGenerator<NestedMemberAssignment>
                    .Generate(member, out parameterTypes, parent);

            var item = new NestedMemberAssignment {Nested = new NestedMemberAssignment.NestedType()};
            fn(item, "P1", 55);
            Assert.Equal("P1,55", item.Nested.Complex.ToString());
        }
    }
}
