using MyNUnit;
using MyNUnit.Attributes;

namespace TestSuitProject;

public class BeforeAndAfterMethods
{
    private bool firstIndicator = true;

    private bool secondIndicator = false;

    [Before]
    public void FirstChangeIndicator()
    {
        this.firstIndicator = !this.firstIndicator;
    }

    [Test]
    public void ExpectedToPassTest()
    {
        if (this.firstIndicator || this.secondIndicator)
        {
            throw new InvalidOperationException();
        }
    }

    [Test]
    public void ExpectedToFailTest()
    {
        if (!(this.firstIndicator || this.secondIndicator))
        {
            throw new InvalidOperationException();
        }
    }

    [After]
    public void SecondChangeIndicator()
    {
        this.secondIndicator = !this.secondIndicator;
    }
}
