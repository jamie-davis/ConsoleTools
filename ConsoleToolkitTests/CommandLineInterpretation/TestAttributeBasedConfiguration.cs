using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    //Disambiguate the Description attribute
    using DescriptionAttribute = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;

    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestAttributeBasedConfiguration
    {


    }
}
