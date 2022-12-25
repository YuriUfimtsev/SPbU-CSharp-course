namespace MyNUnit.Information;

public class TestInfo
{
    public TestInfo(string name, TestStatus.Status executionResult, string report)
        : this(name, 0, executionResult, report)
    {
    }

    public TestInfo(string name, long duration, TestStatus.Status executionResult)
        : this(name, duration, executionResult, string.Empty)
    {
    }

    public TestInfo(string name, long duration, TestStatus.Status executionResult, string report)
    {
        this.Name = name;
        this.Duration = duration;
        this.ExecutionResult = executionResult;
        this.Report = report;
    }

    public string Name { get; }

    public long Duration { get; }

    public TestStatus.Status ExecutionResult { get; }

    public string Report { get; }
}
