namespace MyNUnit.Information;

public class TestClassInfo
{
    public TestClassInfo(string testClassName)
    {
        this.TestClassName = testClassName;
        this.TestsInfo = new ();
        this.BeforeElementsInfo = new ();
        this.AfterElementsInfo = new ();
        this.BeforeClassElementsInfo = new ();
        this.AfterClassElementsInfo = new ();
    }

    public string TestClassName { get; }

    public long Duration { get; set; }

    public List<TestInfo> TestsInfo { get; private set; }

    public List<TestSuitElementInfo> BeforeElementsInfo { get; private set; }

    public List<TestSuitElementInfo> AfterElementsInfo { get; private set; }

    public List<TestSuitElementInfo> BeforeClassElementsInfo { get; private set; }

    public List<TestSuitElementInfo> AfterClassElementsInfo { get; private set; }

    public void AddTest(TestInfo testInfo) => this.TestsInfo.Add(testInfo);

    public void AddTestSuitElement(TestSuitElementInfo testSuitElementInfo, TestSuitElements.TestSuitElementType type)
    {
        var suitableStorage = type switch
        {
            TestSuitElements.TestSuitElementType.Before => this.BeforeElementsInfo,
            TestSuitElements.TestSuitElementType.After => this.AfterElementsInfo,
            TestSuitElements.TestSuitElementType.BeforeClass => this.BeforeClassElementsInfo,
            TestSuitElements.TestSuitElementType.AfterClass => this.BeforeClassElementsInfo,
            _ => throw new NotImplementedException()
        };

        suitableStorage.Add(testSuitElementInfo);
    }
}
