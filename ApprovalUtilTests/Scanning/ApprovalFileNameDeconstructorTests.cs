using ApprovalUtil.Scanning;
using FluentAssertions;

namespace ApprovalUtilTests.Scanning;

public class ApprovalFileNameDeconstructorTests
{
    [Theory]
    [InlineData("TestClass.TestMethod.approved.txt", "TestClass")]
    [InlineData("TestClass.TestMethod.received.txt", "TestClass")]
    [InlineData("TestClass.TestMethod.received", null)]
    [InlineData("TestClass.TestMethod.txt", null)]
    [InlineData("TestClass.approved.txt", null)]
    [InlineData("TestClass.received.txt", null)]
    [InlineData("TestClass..approved.txt", "TestClass")]
    [InlineData(".TestMethod.approved.txt", "")]
    [InlineData("..approved.txt", "")]
    public void TestClassNameIsExtractedFromFileName(string fileName, string className)
    {
        //Act
        var (result, _) = ApprovalFileNameDeconstructor.Deconstruct(fileName);

        //Assert
        result.Should().Be(className);
    }

    [Theory]
    [InlineData("TestClass.TestMethod.approved.txt", "TestMethod")]
    [InlineData("TestClass.TestMethod.received.txt", "TestMethod")]
    [InlineData("TestClass.TestMethod.received", null)]
    [InlineData("TestClass.TestMethod.txt", null)]
    [InlineData("TestClass.approved.txt", null)]
    [InlineData("TestClass.received.txt", null)]
    [InlineData("TestClass..approved.txt", "")]
    [InlineData(".TestMethod.approved.txt", "TestMethod")]
    [InlineData("..approved.txt", "")]
    public void TestNameIsExtractedFromFileName(string fileName, string testName)
    {
        //Act
        var (_, result) = ApprovalFileNameDeconstructor.Deconstruct(fileName);

        //Assert
        result.Should().Be(testName);
    }
}