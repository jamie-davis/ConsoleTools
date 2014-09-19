using System;
using ConsoleToolkit.ConsoleIO.Internal;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    public class TestReportExceptionFilter
    {
        [Test]
        public void AnExceptionAddedSetsIndicator()
        {
            //Arrange
            var filter = new ReportExceptionFilter();

            Exception testException;
            try { throw new NotImplementedException(); }
            catch (Exception e)
            {
                testException = e;
            }

            //Act
            filter.AddException(testException);

            //Assert
            Assert.That(filter.ExceptionCaptured, Is.True);
        }

        [Test]
        public void DefaultExceptionCapturedIsFalse()
        {
            //Arrange
            var filter = new ReportExceptionFilter();

            //Act

            //Assert
            Assert.That(filter.ExceptionCaptured, Is.False);
        }

        [Test]
        public void AddedExceptionIsCaptured()
        {
            //Arrange
            var filter = new ReportExceptionFilter();

            Exception testException;
            try { throw new NotImplementedException(); }
            catch (Exception e)
            {
                testException = e;
            }

            //Act
            filter.AddException(testException);

            //Assert
            Assert.That(filter.Exception, Is.SameAs(testException));
        }
    }
}
