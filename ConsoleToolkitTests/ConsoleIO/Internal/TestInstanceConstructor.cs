using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.ConsoleIO.Internal;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
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

        [Test]
        public void MakeUsingConstructorReturnsCorrectType()
        {
            var props = MakeProps<TargetTypeWithConstructor>(new object[] {"A", 10, 45.5});
            var instance = InstanceConstructor<TargetTypeWithConstructor>.MakeInstanceUsingConstructor(props);
            Assert.That(instance, Is.InstanceOf<TargetTypeWithConstructor>());
        }

        [Test]
        public void MakeUsingConstructorCreatesInstance()
        {
            var props = MakeProps<TargetTypeWithConstructor>(new object[] {"A", 10, 45.5});
            var instance = InstanceConstructor<TargetTypeWithConstructor>.MakeInstanceUsingConstructor(props);
            Assert.That(instance.ToString(), Is.EqualTo("A,10,45.5"));
        }

        [Test]
        public void MakeUsingPropertySettersCreatesInstance()
        {
            var props = MakeProps<TargetTypeWithSetters>(new object[] {"A", 10, 45.5});
            var instance = InstanceConstructor<TargetTypeWithSetters>.MakeInstanceUsingMemberSetters(props);
            Assert.That(instance.ToString(), Is.EqualTo("A,10,45.5"));
        }

        [Test]
        public void MakeUsingPropertySettersReturnsCorrectType()
        {
            var props = MakeProps<TargetTypeWithSetters>(new object[] {"A", 10, 45.5});
            var instance = InstanceConstructor<TargetTypeWithSetters>.MakeInstanceUsingMemberSetters(props);
            Assert.That(instance, Is.InstanceOf<TargetTypeWithSetters>());
        }

        [Test]
        public void MakeUsingConstructorThrowsIfConstructorNotFound()
        {
            var bogusProp = typeof (TargetTypeWithConstructor).GetProperty("V1");
            var props = MakeProps<TargetTypeWithConstructor>(new object[] { "A", 10, 45.5 }).Concat(new [] {new PropertySource {Property = bogusProp, Value = 45}});
            TestDelegate call = () => InstanceConstructor<TargetTypeWithConstructor>.MakeInstanceUsingConstructor(props);
            Assert.Throws<InstanceConstructor<TargetTypeWithConstructor>.NoConstructorWithParametersMathingPropertyFound>(call);
        }

        [Test]
        public void MakeInstanceWithConstructorReturnsCorrectType()
        {
            var props = MakeProps<TargetTypeWithConstructor>(new object[] { "A", 10, 45.5 });
            var instance = InstanceConstructor<TargetTypeWithConstructor>.MakeInstance(props);
            Assert.That(instance, Is.InstanceOf<TargetTypeWithConstructor>());
        }

        [Test]
        public void MakeInstanceWithConstructorCreatesInstance()
        {
            var props = MakeProps<TargetTypeWithConstructor>(new object[] { "A", 10, 45.5 });
            var instance = InstanceConstructor<TargetTypeWithConstructor>.MakeInstance(props);
            Assert.That(instance.ToString(), Is.EqualTo("A,10,45.5"));
        }

        [Test]
        public void MakeInstanceWithSettersReturnsCorrectType()
        {
            var props = MakeProps<TargetTypeWithSetters>(new object[] { "A", 10, 45.5 });
            var instance = InstanceConstructor<TargetTypeWithSetters>.MakeInstance(props);
            Assert.That(instance, Is.InstanceOf<TargetTypeWithSetters>());
        }

        [Test]
        public void MakeInstanceWithSettersCreatesInstance()
        {
            var props = MakeProps<TargetTypeWithSetters>(new object[] { "A", 10, 45.5 });
            var instance = InstanceConstructor<TargetTypeWithSetters>.MakeInstance(props);
            Assert.That(instance.ToString(), Is.EqualTo("A,10,45.5"));
        }

        private IEnumerable<PropertySource> MakeProps<T>(object[] objects)
        {
            return typeof (T).GetProperties()
                .Select((p, i) => new PropertySource {Property = p, Value = objects[i]})
                .ToList();
        }
    }
}