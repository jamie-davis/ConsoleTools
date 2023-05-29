using ApprovalUtil.Scanning;

namespace ApprovalUtilTests.TestUtilities;

internal static class TestResultObjectMother
{
    public static TestOutputGenerator GenerateTests()
    {
        return GenerateSet(DefaultPassingTestsSet);
    }

    public static IDisposable GenerateTests(out List<ApprovalTestResult> scanResult)
    {
        return GenerateAndScanTests(DefaultPassingTestsSet, out scanResult);
    }

    public static TestOutputGenerator GenerateTestsWithFailures()
    {
        return GenerateSet(DefaultFailingTestsSet);
    }

    public static IDisposable GenerateTestsWithFailures(out List<ApprovalTestResult> scanResult)
    {
        return GenerateAndScanTests(DefaultFailingTestsSet, out scanResult);
    }

    private static void DefaultPassingTestsSet(TestOutputGenerator generator)
    {
        generator.MakeTest("TestClass1", "TestOne");
        generator.MakeTest("TestClass1", "TestTwo", true);
        generator.MakeTest("TestClass1", "TestThree");
        generator.MakeTest("TestClass2", "TestOne", true);
    }

    private static void DefaultFailingTestsSet(TestOutputGenerator generator)
    {
        generator.MakeTest("TestClass1", "TestOne");
        generator.MakeTest("TestClass1", "TestTwo", true);
        generator.MakeTest("TestClass1", "TestThree");
        generator.MakeTest("TestClass1", "TestFour", createFailed:true);
        generator.MakeTest("TestClass2", "TestOne", true);
        generator.MakeTest("TestClass2", "TestTwo", createFailed:true);
        generator.MakeTest("TestClass2", "NewTest", createNew: true);
    }

    private static IDisposable GenerateAndScanTests(Action<TestOutputGenerator> MakeTests, out List<ApprovalTestResult> scanResult)
    {
        TestOutputGenerator? generator = null;
        try
        {
            generator = GenerateSet(MakeTests);
            scanResult = TestResultScanner.Scan(generator.FolderPath);
            return generator;
        }
        catch
        {
            generator?.Dispose();
            throw;
        }
    }

    private static TestOutputGenerator GenerateSet(Action<TestOutputGenerator> MakeTests)
    {
        TestOutputGenerator? generator = null;
        try
        {
            generator = new();
            MakeTests(generator);
            return generator;
        }
        catch
        {
            generator?.Dispose();
            throw;
        }
    }
}