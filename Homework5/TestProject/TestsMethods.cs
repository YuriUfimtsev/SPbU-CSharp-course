using MyNUnit;
using MyNUnit.Attributes;

namespace TestSuitProject;

public class TestsMethods
{
    [Test]
    public void FirstPassedTest()
    {
    }

    [Test]
    public void FailedTest()
    {
        throw new InvalidDataException();
    }

    [Test]
    public void SecondPassedTest()
    {
    }

}