using MyNUnit;
using MyNUnit.Attributes;

namespace DetailedTestsProject;

public class TestsMethods
{
    [Test("The reason is not important")]
    public void IgnoredTest()
    {
    }

    [Test(typeof(InvalidDataException))]
    public void ExpectedExceptionPassedTest()
    {
        throw new InvalidDataException();
    }

    [Test(typeof(InvalidDataException))]
    public void ExpectedExceptionFailedTest()
    {
        throw new InvalidOperationException();
    }

    [Test(typeof(InvalidDataException), "no matter")]
    public void ExpectedExceptionIgnoredTest()
    {
        throw new InvalidCastException();
    }
}