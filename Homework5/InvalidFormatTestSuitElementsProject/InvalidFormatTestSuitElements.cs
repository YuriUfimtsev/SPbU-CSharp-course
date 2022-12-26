using MyNUnit;
using MyNUnit.Attributes;

namespace InvalidFormatTestSuitElementsProject;

public class InvalidFormatTestSuitElements
{
    [Test]
    public int IntReturnTypeTest()
    {
        return 3;
    }

    [Test]
    public void HavingOneParameterTest(string reasonWhy)
    {
    }

    [Test]
    public void StandartTest()
    {
    }

    [Before]
    public int IntReturnTypeBeforeMethod()
    {
        return 555;
    }

    [AfterClass]
    public void NonStaticAfterClassMethod()
    {
    }
}