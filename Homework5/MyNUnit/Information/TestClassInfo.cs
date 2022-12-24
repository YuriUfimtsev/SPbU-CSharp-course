namespace MyNUnit.Information;

public class TestClassInfo
{
    public string TestClassName { get; }

    public int Duration { get; private set; }

    public List<TestInfo> TestsInfo { get; private set; }

    public List<TestSuitElementInfo> BeforeElementsInfo { get; private set; }

    public List<TestSuitElementInfo> AfterElementsInfo { get; private set; }

    public List<TestSuitElementInfo> BeforeClassElementsInfo { get; private set; }

    public List<TestSuitElementInfo> AfterClassElementsInfo { get; private set; }

    public void Add()
    {
    }
}
