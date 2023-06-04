using System;
using System.Reflection;
using ConsoleToolkit.Utilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.Utilities
{
    public class TestMethodInvoker
    {
        #region Types for test

        class Item
        {
            public string StringMethod(int n, string s)
            {
                return string.Format("Int: {0}, String: {1}", n, s);
            }

            public string NoParamMethod()
            {
                return "No params";
            }

            public string ThrowMethod(int n, string s)
            {
                throw new ArgumentException("test");
            }

            public void VoidMethod(int n, string s)
            {
                
            }

            public static string StaticStringMethod(int n, string s)
            {
                return string.Format("Int: {0}, String: {1}", n, s);
            }
        }

        #endregion

        [Fact]
        public void MemberMethodCanBeCalled()
        {
            //Arrange
            var item = new Item();
            var method = item.GetType().GetMethod("StringMethod");

            //Act
            var result = MethodInvoker.Invoke(method, item, new object[] { 55, "bob" }) as string;

            //Assert
            Assert.Equal(item.StringMethod(55, "bob"), result);
        }

        [Fact]
        public void NoParamMethodCanBeCalledWithNoParameterArray()
        {
            //Arrange
            var item = new Item();
            var method = item.GetType().GetMethod("NoParamMethod");

            //Act
            var result = MethodInvoker.Invoke(method, item, null) as string;

            //Assert
            Assert.Equal("No params", result);
        }

        [Fact]
        public void VoidMethodCanBeCalled()
        {
            //Arrange
            var item = new Item();
            var method = item.GetType().GetMethod("VoidMethod");

            //Act
            MethodInvoker.Invoke(method, item, new object[] { 55, "bob" });

            //Assert
            //We expect no assertion
        }

        [Fact]
        public void StaticMethodCanBeCalled()
        {
            //Arrange
            var item = new Item();
            var method = item.GetType().GetMethod("StaticStringMethod", BindingFlags.Static | BindingFlags.Public);

            //Act
            MethodInvoker.Invoke(method, null, new object[] { 55, "bob" });

            //Assert
            //We expect no assertion
        }

        [Fact]
        public void ExceptionThrownInMethodEmergesIntact()
        {
            //Arrange
            var item = new Item();
            var method = item.GetType().GetMethod("ThrowMethod");

            //Act
            Action action = () => MethodInvoker.Invoke(method, item, 55, "bob");
            action.Should().Throw<ArgumentException>().Where(e => e.Message == "test");
        }
    }
}
