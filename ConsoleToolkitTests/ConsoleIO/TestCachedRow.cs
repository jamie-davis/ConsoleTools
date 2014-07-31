using System;
using System.Linq;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestCachedRow
    {
        private static readonly DateTime TestDate = DateTime.Parse("2014-06-11 20:36");

        #region Types for test
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local

        class Simple
        {
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

        [SetUp]
        public void SetUp()
        {
            SetUpTests.OverrideCulture();
        }

        [Test]
        public void RowContainsAllColumns()
        {
            var row = new CachedRow<Simple>(10);
            Assert.That(row.Columns.Select(c => c.Property.Name).JoinWith(","), Is.EqualTo("Number,StringData,Date"));
        }

        [Test]
        public void ColumnsHaveExpectedData()
        {
            var row = new CachedRow<Simple>(10);
            Assert.That(Format(row), Is.EqualTo("Number = 10,StringData = string 10,Date = 11/06/2014 20:36:00"));
        }

        private string Format<T>(CachedRow<T> row)
        {
            return row.Columns.Select(c => string.Format("{0} = {1}", c.Property.Name, c.Value).TrimEnd()).JoinWith(",");
        }
    }
}