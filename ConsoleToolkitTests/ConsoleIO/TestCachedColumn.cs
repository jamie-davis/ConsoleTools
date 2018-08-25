using System;
using System.Reflection;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCachedColumn
    {
        private static readonly DateTime TestDate = DateTime.Parse("2014-06-11 21:20");

        #region Types for test
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local

        class Simple
        {
            public static readonly PropertyInfo NumberProp = typeof (Simple).GetProperty("Number");

            public int Number { get; set; }
            public string StringData { get; set; }
            public DateTime Date { get; set; }

            public Simple(int number, string stringData = null, DateTime date = default(DateTime))
            {
                Number = number;
                StringData = stringData ?? string.Format("string {0}", number);
                Date = date == default(DateTime) ? TestDate : date;
            }

            public static implicit operator Simple(int n)
            {
                return new Simple(n);
            }
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore MemberCanBePrivate.Local
        #endregion

        [Test]
        public void ValueIsFormatted()
        {
            var format = new ColumnFormat("Num", Simple.NumberProp.PropertyType);
            format.SetActualWidth(5);
            var col = new CachedColumn(Simple.NumberProp, 50);
            Assert.That(col.Format(format), Is.EqualTo("50"));
        }
    }
}