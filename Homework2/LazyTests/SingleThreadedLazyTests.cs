namespace LazyTests;

using System;
using Lazy;

public class SingleThreadedLazyTests
{
    private static Func<object>[] valueTypeFunctions =
    {
        () => TestFunctions.CalculateRandomValue(),
        () => TestFunctions.GuessNumber(),
    };

    private static Func<object>[] referenceTypeFunctions =
    {
        () => TestFunctions.ÑoncatenateString(),
        () => TestFunctions.CreateNewObject(),
    };

    private static Func<object?>[] nullTypeFunction =
    {
         () => TestFunctions.ReturnNull(),
    };

    [TestCaseSource(nameof(GetArrayWithLazyObjects), new object[] { true, false })]
    public void CompareValueTypeFunctionsResultsTest(ILazy<object> lazy)
    {
        var firstCalculation = lazy.Get();
        var secondCalculation = lazy.Get();
        var thirdCalculation = lazy.Get();
        Assert.That(firstCalculation, Is.EqualTo(secondCalculation));
        Assert.That(firstCalculation, Is.EqualTo(thirdCalculation));
    }

    [TestCaseSource(nameof(GetArrayWithLazyObjects), new object[] { false, false })]
    public void CompareReferenceTypeFunctionsResultsTest(ILazy<object> lazy)
    {
        var firstCalculation = lazy.Get();
        var secondCalculation = lazy.Get();
        var thirdCalculation = lazy.Get();
        Assert.That(firstCalculation, Is.SameAs(secondCalculation));
        Assert.That(firstCalculation, Is.SameAs(thirdCalculation));
    }

    [TestCaseSource(nameof(GetArrayWithLazyObjects), new object[] { false, true })]
    public void NullReturnTypeTest(ILazy<object> lazy)
    {
        var firstCalculation = lazy.Get();
        var secondCalculation = lazy.Get();
        var thirdCalculation = lazy.Get();
        Assert.That(firstCalculation, Is.SameAs(secondCalculation));
        Assert.That(firstCalculation, Is.SameAs(thirdCalculation));
    }

    private static TestCaseData[] GetArrayWithLazyObjects(bool areValueTypeFunctions, bool isNullFunction)
    {
        var functionsArray = nullTypeFunction;
        if (!isNullFunction)
        {
            functionsArray = areValueTypeFunctions ? valueTypeFunctions : referenceTypeFunctions;
        }

        TestCaseData[] lazyObjectsArray = new TestCaseData[functionsArray.Length * 2];
        var (i, j) = (0, 0);
        while (i < (functionsArray.Length * 2))
        {
            lazyObjectsArray[i] = new TestCaseData(new SingleThreadedLazy<object?>(functionsArray[j]));
            lazyObjectsArray[i + 1] = new TestCaseData(new MultiThreadedLazy<object?>(functionsArray[j]));
            i += 2;
            ++j;
        }

        return lazyObjectsArray;
    }
}