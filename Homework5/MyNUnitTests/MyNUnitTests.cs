using MyNUnit;
using MyNUnit.Information;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace MyNUnitTests;

public class MyNUnitTests
{
    private static readonly string pathToTestSuitProjectDll = "../../../TestSuit";
    private static readonly string pathToDetailedTestsProjectDll = "../../../DetailedTests";
    private static readonly string pathToInvalidFormatTestSuitElementsProjectDll = "../../../InvalidFormatTestSuitElements";

    [Test]
    public void TestSuitProjectTest()
    {
        var information = TestRunner.RunTests(pathToTestSuitProjectDll);
        var expectedInformation = new List<TestClassInfo>();
        var firstExpectedTestClassInfo = new TestClassInfo("TestsMethods");
        firstExpectedTestClassInfo.AddTest(new TestInfo("FirstPassedTest", TestStatus.Status.Passed));
        firstExpectedTestClassInfo.AddTest(new TestInfo("FailedTest", TestStatus.Status.Failed));
        firstExpectedTestClassInfo.AddTest(new TestInfo("SecondPassedTest", TestStatus.Status.Passed));
        var secondExpectedTestClassInfo = new TestClassInfo("BeforeAndAfterMethods");
        secondExpectedTestClassInfo.AddTest(new TestInfo("ExpectedToPassTest", TestStatus.Status.Passed));
        //secondExpectedTestClassInfo.AddTest(new TestInfo("ExpectedToFailTest", TestStatus.Status.Failed));
        expectedInformation.Add(firstExpectedTestClassInfo);
        expectedInformation.Add(secondExpectedTestClassInfo);
        Assert.That(CompareAssembliesInfo(information, expectedInformation), Is.EqualTo(0));
    }

    [Test]
    public void DetailedTestsProjectTest()
    {
        var information = TestRunner.RunTests(pathToDetailedTestsProjectDll);
        var expectedInformation = new List<TestClassInfo>();
        var expectedTestClassInfo = new TestClassInfo("TestsMethods");
        expectedTestClassInfo.AddTest(new TestInfo("IgnoredTest", TestStatus.Status.Ignored));
        expectedTestClassInfo.AddTest(new TestInfo("ExpectedExceptionPassedTest", TestStatus.Status.Passed));
        expectedTestClassInfo.AddTest(new TestInfo("ExpectedExceptionFailedTest", TestStatus.Status.Failed));
        expectedTestClassInfo.AddTest(new TestInfo("ExpectedExceptionIgnoredTest", TestStatus.Status.Ignored));
        expectedInformation.Add(expectedTestClassInfo);
        Assert.That(CompareAssembliesInfo(information, expectedInformation), Is.EqualTo(0));
    }

    [Test]
    public void InvalidFormatTestSuitElementsProjectTest()
    {
        var information = TestRunner.RunTests(pathToInvalidFormatTestSuitElementsProjectDll);
        var expectedInformation = new List<TestClassInfo>();
        var expectedTestClassInfo = new TestClassInfo("InvalidFormatTestSuitElements");
        expectedTestClassInfo.AddTest(new TestInfo("StandartTest", TestStatus.Status.Passed));
        var expectedInvalidTestsNames = new List<string> { "IntReturnTypeTest", "HavingOneParameterTest" };
        var expectedInvalidTestSuitElementsNames = new List<string> { "IntReturnTypeBeforeMethod", "NonStaticAfterClassMethod" };
        expectedTestClassInfo.AddIncorrectTestsNames(expectedInvalidTestsNames);
        expectedTestClassInfo.AddIncorrectTestSuitElementsNames(expectedInvalidTestSuitElementsNames);
        expectedInformation.Add(expectedTestClassInfo);
        Assert.That(CompareAssembliesInfo(information, expectedInformation), Is.EqualTo(0));
    }

    private static int CompareAssembliesInfo(List<TestClassInfo> actual, List<TestClassInfo> expected)
    {
        if (actual.Count != expected.Count)
        {
            return -1;
        }

        var testClassInfoComparison = new Comparison<TestClassInfo>((firstTestClassInfo, secondTestClassInfo)
            => string.Compare(firstTestClassInfo.TestClassName, secondTestClassInfo.TestClassName));
        actual.Sort(testClassInfoComparison);
        expected.Sort(testClassInfoComparison);
        for (var i = 0; i < expected.Count; ++i)
        {
            if (CompareTestsClassesInfo(actual[i], expected[i]) != 0)
            {
                return -1;
            }
        }

        return 0;

    }

    private static int CompareTestsClassesInfo(TestClassInfo actual, TestClassInfo expected)
    {
        if (actual.TestClassName != expected.TestClassName || actual.TestsInfo.Count != expected.TestsInfo.Count
            || actual.InvalidTestsNames.Count != expected.InvalidTestsNames.Count
            || actual.InvalidTestSuitElementsNames.Count != expected.InvalidTestSuitElementsNames.Count)
        {
            return -1;
        }

        if (CompareTestsInfo(actual.TestsInfo, expected.TestsInfo) != 0)
        {
            return -1;
        }

        actual.InvalidTestsNames.Sort();
        expected.InvalidTestsNames.Sort();
        if (!actual.InvalidTestsNames.SequenceEqual(expected.InvalidTestsNames))
        {
            return -1;
        }

        actual.InvalidTestSuitElementsNames.Sort();
        expected.InvalidTestSuitElementsNames.Sort();
        if (!actual.InvalidTestSuitElementsNames.SequenceEqual(expected.InvalidTestSuitElementsNames))
        {
            return -1;
        }

        return 0;
    }

    private static int CompareTestsInfo(IEnumerable<TestInfo> actual, IEnumerable<TestInfo> expected)
    {
        var actualList = actual.ToList();
        var expectedList = expected.ToList();
        if (actualList.Count != expectedList.Count)
        {
            return -1;
        }

        var testInfoComparison = new Comparison<TestInfo>((firstTestInfo, secondTestInfo)
            => string.Compare(firstTestInfo.Name, secondTestInfo.Name));
        actualList.Sort(testInfoComparison);
        expectedList.Sort(testInfoComparison);
        for (var i = 0; i < expectedList.Count; ++i)
        {
            if (actualList[i].Name != expectedList[i].Name || actualList[i].ExecutionResult != expectedList[i].ExecutionResult)
            {
                return -1;
            }
        }

        return 0;
    }
}