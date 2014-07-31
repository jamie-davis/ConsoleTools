using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleToolkit.ApplicationStyles.Internals;
using NUnit.Framework;

namespace ConsoleToolkitTests.ApplicationStyles.Internals
{
    [TestFixture]
    public class TestMethodParameterInjector
    {
        private MethodInfo _method1;
        private MethodInfo _method2;
        
        #region Test methods
// ReSharper disable UnusedMember.Global
        public void TestMethod1(TestMethodParameterInjector inj) {}
        public void TestMethod2(TestMethodParameterInjector inj, int n, string s) {}
// ReSharper restore UnusedMember.Global
        #endregion

        [SetUp]
        public void SetUp()
        {
            _method1 = GetType().GetMethod("TestMethod1");
            _method2 = GetType().GetMethod("TestMethod2");
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
    }
}
