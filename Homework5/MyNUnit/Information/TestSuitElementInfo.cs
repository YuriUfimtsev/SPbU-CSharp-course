namespace MyNUnit.Information;

public class TestSuitElementInfo
{
    public string Name { get; }

    public TestSuitElements.TestSuitElementType Type { get; }

    public bool ExecutionResult { get; }

    public string ErrorReport { get; }
}
