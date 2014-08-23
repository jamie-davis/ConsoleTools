using System;
using System.Collections.Generic;
using System.Reflection;
using ConsoleToolkit.ApplicationStyles.Internals;
using NUnit.Framework;

namespace ConsoleToolkitTests.ApplicationStyles.Internals
{
    [TestFixture]
    public class TestMethodParameterInjector
    {
        private MethodInfo _method1;
        private MethodInfo _method2;
        private MethodInfo _method3;

        // ReSharper disable UnusedMember.Global
        #region Types for test
        public interface ITestInterface
        {
            
        }

        class TestImpl : ITestInterface
        {
            
        }
        #endregion

        #region Test methods
        public void TestMethod1(TestMethodParameterInjector inj) {}
        public void TestMethod2(TestMethodParameterInjector inj, int n, string s) {}
        public void TestMethod3(TestMethodParameterInjector inj, ITestInterface t) {}
        #endregion
        // ReSharper restore UnusedMember.Global

        [SetUp]
        public void SetUp()
        {
            _method1 = GetType().GetMethod("TestMethod1");
            _method2 = GetType().GetMethod("TestMethod2");
            _method3 = GetType().GetMethod("TestMethod3");
        }

        [Test]
        public void OneTimeInjectableIsUsed()
        {
            var injector = new MethodParameterInjector(new object[]{});

            Assert.That(injector.GetParameters(_method1, new object[] {this}), Is.EqualTo(new object[] {this}));
        }

        [Test]
        public void ConstructorProvidedInjectableIsUsed()
        {
            var injector = new MethodParameterInjector(new object[]{this});

            Assert.That(injector.GetParameters(_method1, new object[] {}), Is.EqualTo(new object[] {this}));
        }

        [Test]
        public void MultipleParametersAreInjected()
        {
            var injector = new MethodParameterInjector(new object[]{5, "string"});

            Assert.That(injector.GetParameters(_method2, new object[] {this}), Is.EqualTo(new object[] {this, 5, "string"}));
        }

        [Test, ExpectedException]
        public void AnExceptionIsThrownIfTheParametersCannotBeInjected()
        {
            var injector = new MethodParameterInjector(new object[]{5});

            injector.GetParameters(_method2, new object[] {this});
        }

        [Test]
        public void CanSupplyReturnsTrueForTypeThatCanBeProvided()
        {
            var injector = new MethodParameterInjector(new object[]{"string", 5, this});
            Assert.That(injector.CanSupply(GetType()), Is.True);
        }

        [Test]
        public void CanSupplyReturnsFalseForTypeThatCannotBeProvided()
        {
            var injector = new MethodParameterInjector(new object[]{"string", 5});
            Assert.That(injector.CanSupply(GetType()), Is.False);
        }

        [Test]
        public void CanSupplyReturnsTrueForTypesWithSpecifiedInstances()
        {
            var injector = new MethodParameterInjector(new object[]{"string", 5}, new [] {new KeyValuePair<Type, object>(typeof(ITestInterface), new TestImpl()) });
            Assert.That(injector.CanSupply(typeof(ITestInterface)), Is.True);
        }

        [Test]
        public void CanSupplyASpecificInstanceForPredefinedParameterTypes()
        {
            var testImpl = new TestImpl();
            var injector = new MethodParameterInjector(new object[] { "string", 5 },
                new[] { new KeyValuePair<Type, object>(typeof(ITestInterface), testImpl) });

            injector.GetParameters(_method3, new object[] { this });
            Assert.That(injector.GetParameters(_method3, new object[] { this })[1], Is.SameAs(testImpl));
        }

        [Test]
        public void SpecificInstancesCanBeInjectedAfterConstruction()
        {
            var testImpl = new TestImpl();
            var injector = new MethodParameterInjector(new object[] { "string", 5 });

            injector.AddInstance<ITestInterface>(testImpl);

            injector.GetParameters(_method3, new object[] { this });
            Assert.That(injector.GetParameters(_method3, new object[] { this })[1], Is.SameAs(testImpl));
        }
    }
}
