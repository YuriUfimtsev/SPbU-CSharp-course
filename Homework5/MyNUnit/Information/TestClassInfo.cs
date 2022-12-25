namespace MyNUnit.Information;

public class TestClassInfo
{
    public TestClassInfo(string testClassName)
    {
        this.TestClassName = testClassName;
        this.TestsInfo = new ();
        this.InvalidTestsNames = new ();
        this.InvalidTestSuitElementsNames = new ();
    }

    public string TestClassName { get; }

    public long Duration { get; set; }

    public List<TestInfo> TestsInfo { get; private set; }

    public List<string> InvalidTestsNames { get; private set; }

    public List<string> InvalidTestSuitElementsNames { get; private set; }

    public void AddTest(TestInfo testInfo) => this.TestsInfo.Add(testInfo);

    public void AddIncorrectTestsNames(List<string> invalidTestsNames) => this.InvalidTestsNames = invalidTestsNames;

    public void AddIncorrectTestSuitElementsNames(List<string> invalidTestSuitElementsNames)
        => this.InvalidTestSuitElementsNames = invalidTestSuitElementsNames;
}
