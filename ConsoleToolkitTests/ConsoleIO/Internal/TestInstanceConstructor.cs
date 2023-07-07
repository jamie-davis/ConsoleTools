using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.ConsoleIO.Internal;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    public class TestInstanceConstructor
    {
        #region Types for test

        class PropertySource : IPropertySource
        {
            public PropertyInfo Property { get; set; }
            public object Value { get; set; }
        }

        class TargetTypeWithConstructor
        {
            public string V1 { get; private set; }
            public int V2 { get; private set; }
            public double V3 { get; private set; }

            public TargetTypeWithConstructor(string v1, int v2, double v3)
            {
                V1 = v1;
                V2 = v2;
                V3 = v3;
            }

            public override string ToString()
            {
                return string.Format("{0},{1},{2}", V1, V2, V3);
            }
        }

        class TargetTypeWithSetters
        {
            public string V1 { get; set; }
            public int V2 { get; set; }
            public double V3 { get; set; }

            public override string ToString()
            {
                return string.Format("{0},{1},{2}", V1, V2, V3);
            }
        }
        #endregion

        [Fact]
        public void MakeUsingConstructorReturnsCorrectType()
        {
            var props = MakeProps<TargetTypeWithConstructor>(new object[] {"A", 10, 45.5});
            var instance = InstanceConstructor<TargetTypeWithConstructor>.MakeInstanceUsingConstructor(props);
            instance.Should().BeOfType<TargetTypeWithConstructor>();
        }

        [Fact]
        public void MakeUsingConstructorCreatesInstance()
        {
            var props = MakeProps<TargetTypeWithConstructor>(new object[] {"A", 10, 45.5});
            var instance = InstanceConstructor<TargetTypeWithConstructor>.MakeInstanceUsingConstructor(props);
            Assert.Equal("A,10,45.5", instance.ToString());
        }

        [Fact]
        public void MakeUsingPropertySettersCreatesInstance()
        {
            var props = MakeProps<TargetTypeWithSetters>(new object[] {"A", 10, 45.5});
            var instance = InstanceConstructor<TargetTypeWithSetters>.MakeInstanceUsingMemberSetters(props);
            Assert.Equal("A,10,45.5", instance.ToString());
        }

        [Fact]
        public void MakeUsingPropertySettersReturnsCorrectType()
        {
            var props = MakeProps<TargetTypeWithSetters>(new object[] {"A", 10, 45.5});
            var instance = InstanceConstructor<TargetTypeWithSetters>.MakeInstanceUsingMemberSetters(props);
            instance.Should().BeOfType<TargetTypeWithSetters>();
        }

        [Fact]
        public void MakeUsingConstructorThrowsIfConstructorNotFound()
        {
            var bogusProp = typeof (TargetTypeWithConstructor).GetProperty("V1");
            var props = MakeProps<TargetTypeWithConstructor>(new object[] { "A", 10, 45.5 }).Concat(new [] {new PropertySource {Property = bogusProp, Value = 45}});
            Action call = () => InstanceConstructor<TargetTypeWithConstructor>.MakeInstanceUsingConstructor(props);
            Assert.Throws<InstanceConstructor<TargetTypeWithConstructor>.NoConstructorWithParametersMathingPropertyFound>(call);
        }

        [Fact]
        public void MakeInstanceWithConstructorReturnsCorrectType()
        {
            var props = MakeProps<TargetTypeWithConstructor>(new object[] { "A", 10, 45.5 });
            var instance = InstanceConstructor<TargetTypeWithConstructor>.MakeInstance(props);
            instance.Should().BeOfType<TargetTypeWithConstructor>();
        }

        [Fact]
        public void MakeInstanceWithConstructorCreatesInstance()
        {
            var props = MakeProps<TargetTypeWithConstructor>(new object[] { "A", 10, 45.5 });
            var instance = InstanceConstructor<TargetTypeWithConstructor>.MakeInstance(props);
            Assert.Equal("A,10,45.5", instance.ToString());
        }

        [Fact]
        public void MakeInstanceWithSettersReturnsCorrectType()
        {
            var props = MakeProps<TargetTypeWithSetters>(new object[] { "A", 10, 45.5 });
            var instance = InstanceConstructor<TargetTypeWithSetters>.MakeInstance(props);
            instance.Should().BeOfType<TargetTypeWithSetters>();
        }

        [Fact]
        public void MakeInstanceWithSettersCreatesInstance()
        {
            var props = MakeProps<TargetTypeWithSetters>(new object[] { "A", 10, 45.5 });
            var instance = InstanceConstructor<TargetTypeWithSetters>.MakeInstance(props);
            Assert.Equal("A,10,45.5", instance.ToString());
        }

        private IEnumerable<PropertySource> MakeProps<T>(object[] objects)
        {
            return typeof (T).GetProperties()
                .Select((p, i) => new PropertySource {Property = p, Value = objects[i]})
                .ToList();
        }
    }
}