using System;
using System.Collections.Generic;
using System.Reflection;
using ConsoleToolkit.ApplicationStyles.Internals;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ApplicationStyles.Internals
{
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

        public TestMethodParameterInjector()
        {
            _method1 = GetType().GetMethod("TestMethod1");
            _method2 = GetType().GetMethod("TestMethod2");
            _method3 = GetType().GetMethod("TestMethod3");
        }

        [Fact]
        public void OneTimeInjectableIsUsed()
        {
            var injector = new MethodParameterInjector(new object[]{});

            Assert.Equal(new object[] { this }, injector.GetParameters(_method1, new object[] { this }));
        }

        [Fact]
        public void ConstructorProvidedInjectableIsUsed()
        {
            var injector = new MethodParameterInjector(new object[]{this});

            Assert.Equal(new object[] { this }, injector.GetParameters(_method1, new object[] { }));
        }

        [Fact]
        public void MultipleParametersAreInjected()
        {
            var injector = new MethodParameterInjector(new object[]{5, "string"});

            Assert.Equal(new object[] { this, 5, "string" }, injector.GetParameters(_method2, new object[] { this }));
        }

        [Fact]
        public void AnExceptionIsThrownIfTheParametersCannotBeInjected()
        {
            var action = new Action(() =>
            {
                var injector = new MethodParameterInjector(new object[] {5});

                injector.GetParameters(_method2, new object[] {this});
            });
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void CanSupplyReturnsTrueForTypeThatCanBeProvided()
        {
            var injector = new MethodParameterInjector(new object[]{"string", 5, this});
            injector.CanSupply(GetType()).Should().BeTrue();
        }

        [Fact]
        public void CanSupplyReturnsFalseForTypeThatCannotBeProvided()
        {
            var injector = new MethodParameterInjector(new object[]{"string", 5});
            injector.CanSupply(GetType()).Should().BeFalse();
        }

        [Fact]
        public void CanSupplyReturnsTrueForTypesWithSpecifiedInstances()
        {
            var injector = new MethodParameterInjector(new object[]{"string", 5}, new [] {new KeyValuePair<Type, object>(typeof(ITestInterface), new TestImpl()) });
            injector.CanSupply(typeof(ITestInterface)).Should().BeTrue();
        }

        [Fact]
        public void CanSupplyASpecificInstanceForPredefinedParameterTypes()
        {
            var testImpl = new TestImpl();
            var injector = new MethodParameterInjector(new object[] { "string", 5 },
                new[] { new KeyValuePair<Type, object>(typeof(ITestInterface), testImpl) });

            injector.GetParameters(_method3, new object[] { this });
            injector.GetParameters(_method3, new object[] { this })[1].Should().BeSameAs(testImpl);
        }

        [Fact]
        public void SpecificInstancesCanBeInjectedAfterConstruction()
        {
            var testImpl = new TestImpl();
            var injector = new MethodParameterInjector(new object[] { "string", 5 });

            injector.AddInstance<ITestInterface>(testImpl);

            injector.GetParameters(_method3, new object[] { this });
            injector.GetParameters(_method3, new object[] { this })[1].Should().BeSameAs(testImpl);
        }

        [Fact]
        public void GetParameterReturnsAnInstanceMatchingTheTypeRequested()
        {
            var testImpl = new TestImpl();
            var injector = new MethodParameterInjector(new object[] { "string", 5 });

            injector.AddInstance<ITestInterface>(testImpl);

            injector.GetParameter<ITestInterface>().Should().BeSameAs(testImpl);
        }

        [Fact]
        public void GetParameterReturnsNullForAnUnknownType()
        {
            var injector = new MethodParameterInjector(new object[] { "string", 5 });
            injector.GetParameter<ITestInterface>().Should().BeNull();
        }

        [Fact]
        public void GetParameterReturnsTheDefaultForAnUnknownValueType()
        {
            var injector = new MethodParameterInjector(new object[] { "string", 5 });
            Assert.Equal(default(DateTime), injector.GetParameter<DateTime>());
        }
    }
}
