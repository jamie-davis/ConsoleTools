using System;
using System.Reflection;
using ConsoleToolkit.Utilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.Utilities
{
    [TestFixture]
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

        [Test]
        public void MemberMethodCanBeCalled()
        {
            //Arrange
            var item = new Item();
            var method = item.GetType().GetMethod("StringMethod");

            //Act
            var result = MethodInvoker.Invoke(method, item, new object[] { 55, "bob" }) as string;

            //Assert
            Assert.That(result, Is.EqualTo(item.StringMethod(55, "bob")));
        }

        [Test]
        public void NoParamMethodCanBeCalledWithNoParameterArray()
        {
            //Arrange
            var item = new Item();
            var method = item.GetType().GetMethod("NoParamMethod");

            //Act
            var result = MethodInvoker.Invoke(method, item, null) as string;

            //Assert
            Assert.That(result, Is.EqualTo("No params"));
        }

        [Test]
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

        [Test]
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

        [Test]
        public void ExceptionThrownInMethodEmergesIntact()
        {
            //Arrange
            var item = new Item();
            var method = item.GetType().GetMethod("ThrowMethod");

            //Act
            Assert.That(() => MethodInvoker.Invoke(method, item, 55, "bob"), Throws.TypeOf<ArgumentException>().With.Message.EqualTo("test"));
        }
    }
}
