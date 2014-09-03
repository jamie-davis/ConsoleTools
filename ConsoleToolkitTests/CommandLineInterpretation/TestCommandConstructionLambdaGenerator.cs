using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Utilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    public class TestCommandConstructionLambdaGenerator
    {
        #region Types for test
        //Don't complain about unused things
#pragma warning disable 649
#pragma warning disable 169
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local

        class OptionSet1
        {
            [Option]
            public string OptSet1 { get; set; }
        }

        class OptionSet2
        {
            [Option]
            public string OptSet2 { get; set; }
        }

        class SimpleCommand
        {
            [Option]
            public string Opt1 { get; set; }
        }

        class CommandWithOptionSet
        {
            [Option]
            public string Opt1 { get; set; }

            [OptionSet]
            public OptionSet1 SimpleOptions { get; set; }
        }

        class CommandWithOptionSets
        {
            [Positional]
            public string Pos1 { get; set; }

            [OptionSet]
            public OptionSet1 SimpleOptions1 { get; set; }

            [OptionSet]
            public OptionSet2 SimpleOptions2 { get; set; }
        }

        class CommandWithList
        {
            [Positional]
            public List<string> Repeating { get; set; }

            [Option]
            public List<string> RepeatingOpt { get; set; }

            [OptionSet]
            public OptionSet1 SimpleOptions1 { get; set; }
        }

        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore ClassNeverInstantiated.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
#pragma warning restore 169
#pragma warning restore 649
        #endregion

        private string Describe<T>(T item)
        {
            var optionSets = typeof (T).GetProperties()
                .Where(p => p.GetCustomAttribute<OptionSetAttribute>() != null)
                .ToList();
            return optionSets.Select(o => DescribeOptionSet(item, o)).JoinWith(",");
        }

        private static string DescribeOptionSet<T>(T item, PropertyInfo optionSetProp)
        {
            var value = optionSetProp.GetValue(item);
            var valueText = value == null 
                ? "<null>" 
                : string.Format("instance of {0}", value.GetType().Name);

            return string.Format("{0} = {1}", optionSetProp.Name, valueText);
        }

        [Test]
        public void SimpleCommandIsConstructed()
        {
            var lambda = CommandConstructionLambdaGenerator<SimpleCommand>.Generate();
            var item = lambda();
            Assert.That(item, Is.InstanceOf<SimpleCommand>());
        }

        [Test]
        public void CommandWithOpionSetIsConstructed()
        {
            var lambda = CommandConstructionLambdaGenerator<CommandWithOptionSet>.Generate();
            var item = lambda();
            var description = Describe(item);
            Assert.That(description, Is.EqualTo("SimpleOptions = instance of OptionSet1"));
        }

        [Test]
        public void CommandWithMultipleOpionSetsIsConstructed()
        {
            var lambda = CommandConstructionLambdaGenerator<CommandWithOptionSets>.Generate();
            var item = lambda();
            var description = Describe(item);
            Assert.That(description, Is.EqualTo("SimpleOptions1 = instance of OptionSet1,SimpleOptions2 = instance of OptionSet2"));
        }

        [Test]
        public void ListPositionalIsConstructed()
        {
            var lambda = CommandConstructionLambdaGenerator<CommandWithList>.Generate();
            var item = lambda();
            Assert.That(item.Repeating, Is.Not.Null);
        }

        [Test]
        public void ListOptionIsConstructed()
        {
            var lambda = CommandConstructionLambdaGenerator<CommandWithList>.Generate();
            var item = lambda();
            Assert.That(item.RepeatingOpt, Is.Not.Null);
        }
    }
}