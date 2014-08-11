using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.Utilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.Utilities
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestStackWalker
    {
        class TestClass
        {
            public static string Run()
            {
                return NestedTestClass.Run();
            }

            class NestedTestClass
            {
                public static string Run()
                {
                    //The stack will have all of the types in the call stack, which will give a result that may be fragile.
                    //To restrict the result to something at least somewhat predictable, we put all of the types into a list
                    //and take only the types involved in the test - i.e. eliminate all types in the stack above the test
                    //fixture.
                    var types = StackWalker.StackedTypes().ToList();
                    var lastIndex = types.FindIndex(t => t == typeof (TestStackWalker)) + 1;
                    return types.Take(lastIndex).Select(s => s.Name).JoinWith(",");
                }
            }
        }

        [Test]
        public void AllClassesAreExtracted()
        {
            Assert.That(TestClass.Run(), Is.EqualTo("List`1,Enumerable,NestedTestClass,TestClass,TestStackWalker"));
        }
    }
}
