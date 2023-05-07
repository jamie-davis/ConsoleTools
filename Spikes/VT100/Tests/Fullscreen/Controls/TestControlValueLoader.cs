using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.VisualBasic.CompilerServices;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen;
using VT100.FullScreen.Controls;
using Xunit;

namespace VT100.Tests.Fullscreen.Controls
{
    public class TestControlValueLoader
    {
        #region Types for test

        class DataSource<T> : ILayout
        {
            public T Value { get; set; }

            #region Implementation of ILayout

            public event LayoutUpdated LayoutUpdated;

            #endregion
        }

        private static (ILayout? DataSource, Func<object, object> Getter) MakeTestInstance<T>(T value) 
        {
            var dataSource = new DataSource<T> { Value = value };
            return (dataSource, o => ((DataSource<T>)o).Value);
        }

        private static (ILayout? DataSource, Func<object, object> Getter) MakeTypedTestInstance(Type type, object value)
        {
            var untypedMethod = typeof(TestControlValueLoader).GetMethod(nameof(MakeTestInstance),
                BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(untypedMethod != null);
            var method = untypedMethod.MakeGenericMethod(type);
            return ((ILayout? DataSource, Func<object, object> Getter))method.Invoke(null, new []{ value });
        }
        #endregion
        
        public static IEnumerable<object[]> ValueLoadTestData => new List<object[]>
            {
                new object[] { typeof(string), "abc", "abc" },
                new object[] { typeof(int), 1, "1" },
                new object[] { typeof(double), 1.00, "1" },
                new object[] { typeof(decimal), 1.00M, "1.00" },
                new object[] { typeof(decimal?), null, "" },
            };
        
        [Theory]
        [MemberData(nameof(ValueLoadTestData))]
        public void ValueIsLoadedAsAString(Type dataType, object literal, string expected)
        {
            //Arrange
            var (instance, getter) = MakeTypedTestInstance(dataType, literal);
            
            //Act
            var value = ControlValueLoader.GetString(getter, instance);

            //Assert
            value.Should().Be(expected);
        }

        [Fact]
        public void ANullLayoutReturnsANullValue()
        {
            //Arrange
            var (_, getter) = MakeTestInstance("X");
            
            //Act
            var value = ControlValueLoader.GetString(getter, null);

            //Assert
            value.Should().BeNull();
        }
        
        
    }
}